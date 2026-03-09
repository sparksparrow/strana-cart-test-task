using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.Json;
using task.Configuration;
using task.DbSql;
using task.Dtos;
using task.Entities;
using task.Enums;

namespace task.Services;

public interface ITerminalImportService
{
	Task ImportAsync(CancellationToken cancellationToken);
}

public class TerminalImportService(
		DellinDictionaryDbContext dbContext,
		ILogger<TerminalImportService> logger,
		AppSettings appSettings) : ITerminalImportService
{
	private const string searchString = "дом № ";

	// JsonSerializer с case-insensitive (System.Text.Json)
	private static readonly JsonSerializerOptions JsonOptions = new()
	{
		PropertyNameCaseInsensitive = true
	};

	public async Task ImportAsync(CancellationToken cancellationToken)
	{
		logger.LogInformation("Начало импорта терминалов из {FilePath}", appSettings.TerminalsFilePath);

		try
		{
			// 1. Загружаем терминалы
			var offices = await LoadFromJsonAsync(cancellationToken);
			logger.LogInformation("Загружено {Count} терминалов из JSON", offices.Count);

			await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);

			// 2. Удаляем существующие терминалы
			var deletedCount = await dbContext.Offices.ExecuteDeleteAsync(cancellationToken);
			logger.LogInformation("Удалено {DeletedCount} старых записей", deletedCount);

			// 3. Вставляем новые терминалы
			await dbContext.Offices.AddRangeAsync(offices, cancellationToken);
			await dbContext.SaveChangesAsync(cancellationToken);

			await transaction.CommitAsync(cancellationToken);
			logger.LogInformation("Сохранено {NewCount} новых терминалов", offices.Count);
		}
		catch (OperationCanceledException ex)
		{
			logger.LogWarning(ex, "Импорт терминалов был отменён");

			throw;
		}
		catch (Exception ex)
		{
			logger.LogError(ex, "Ошибка импорта");

			throw;
		}
	}

	private async Task<List<Office>> LoadFromJsonAsync(CancellationToken cancellationToken)
	{
		if (!File.Exists(appSettings.TerminalsFilePath))
		{
			throw new FileNotFoundException($"Файл терминалов не найден: {appSettings.TerminalsFilePath}");
		}

		await using var stream = File.OpenRead(appSettings.TerminalsFilePath);
		var dtos = await JsonSerializer.DeserializeAsync<TerminalJsonRootDto>(stream, JsonOptions, cancellationToken);

		if (dtos is null || dtos.City is null)
		{
			throw new InvalidDataException("Файл терминалов пуст или имеет неверный формат");
		}

		return dtos.City.SelectMany(MapToEntity).ToList();
	}

	private static IEnumerable<Office> MapToEntity(CityDto cityDto)
	{
		if (cityDto is null || cityDto.Terminals is null || cityDto.Terminals.Terminal is null)
		{
			return [];
		}

		var rootCity = cityDto;

		return cityDto.Terminals.Terminal.Select(t => new Office
		{
			Id = int.Parse(t.Id),
			Code = cityDto.Code,
			// думал брал cityDto.Code, но в ТЗ CityCode указан как Int, поэтому взял cityDto.CityId, который тоже int
			CityCode = cityDto.CityId,
			// Не нашел Uuid в файле terminals.json, поэтому взял terminal -> addressCode -> street_code
			Uuid = t.AddressCode?.StreetCode,
			// Не нашел CountryCode в файле terminals.json, поэтому взял Цифровой код России по ISO
			CountryCode = "643",
			// Не совсем понял в каком формате должен храниться WorkTime, поэтому сохранил как JSON - ключ worktables
			WorkTime = JsonSerializer.Serialize(t.Worktables, JsonOptions),
			Type = GetOfficeType(t),
			Coordinates = new Coordinates
			{
				Latitude = double.Parse(t.Latitude, CultureInfo.InvariantCulture),
				Longitude = double.Parse(t.Longitude, CultureInfo.InvariantCulture)
			},
			Address = t.Address is null
				? new Address()
				: new Address
				{
					FullAddress = t.FullAddress,
					City = t.Name,
					Street = t.Address,
					House = GetHouse(t),
					PostalCode = GetPostalCode(t)
				},
			Phones = t.Phones?
				.Select(p => new Phone { PhoneNumber = p.Number, Additional = GetAdditional(p) })
				.ToList() ?? []
		});
	}

	private static OfficeType? GetOfficeType(TerminalDto terminalDto)
	{
		if (terminalDto.IsPvz)
		{
			return OfficeType.PVZ;
		}

		// Надеюсь под офисом имелся ввиду постамат :)
		if (terminalDto.IsOffice)
		{
			return OfficeType.POSTAMAT;
		}

		if (terminalDto.Storage)
		{
			return OfficeType.WAREHOUSE;
		}

		return null;
	}

	private static string GetPostalCode(TerminalDto terminalDto)
	{
		var stringBuilder = new StringBuilder();
		foreach (var ch in terminalDto.FullAddress)
		{
			if (!char.IsDigit(ch))
			{
				break;
			}

			stringBuilder.Append(ch);
		}

		return stringBuilder.ToString();
	}

	private static string GetHouse(TerminalDto terminalDto)
	{
		int index = terminalDto.FullAddress.IndexOf(searchString);
		if (index != -1)
		{
			// Находим начало номера (после "дом № ")
			int startIndex = index + searchString.Length;

			// Ищем конец номера (до запятой или конца строки)
			int endIndex = terminalDto.FullAddress.IndexOf(",", startIndex);
			if (endIndex == -1)
			{
				endIndex = terminalDto.FullAddress.Length;
			}

			// Извлекаем номер дома
			return terminalDto.FullAddress.Substring(startIndex, endIndex - startIndex).Trim();
		}

		return string.Empty;
	}

	private static string? GetAdditional(PhonesDto phonesDto)
	{
		if (!string.IsNullOrEmpty(phonesDto.Comment) && !string.IsNullOrEmpty(phonesDto.Type))
		{
			return $"{phonesDto.Type}, {phonesDto.Comment}";
		}

		if (!string.IsNullOrEmpty(phonesDto.Comment) && string.IsNullOrEmpty(phonesDto.Type))
		{
			return phonesDto.Comment;
		}

		if (string.IsNullOrEmpty(phonesDto.Comment) && !string.IsNullOrEmpty(phonesDto.Type))
		{
			return phonesDto.Type;
		}

		return null!;
	}
}
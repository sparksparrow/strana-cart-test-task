using task.Services;

namespace task;

public class Worker(TimeProvider timeProvider, IServiceProvider serviceProvider) : BackgroundService
{
	private static readonly TimeZoneInfo MoscowTz =
		TimeZoneInfo.FindSystemTimeZoneById("Europe/Moscow");

	protected override async Task ExecuteAsync(CancellationToken stoppingToken)
	{
		using (var scope = serviceProvider.CreateScope())
		{
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<Worker>>();
			logger.LogInformation("Фоновая задача {BackgroundServiceName} запущена. Ожидание следующего запуска в 02:00 MSK...", nameof(Worker));
		}

		while (!stoppingToken.IsCancellationRequested)
		{
			await using var scope = serviceProvider.CreateAsyncScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILogger<Worker>>();
			var importService = scope.ServiceProvider.GetRequiredService<ITerminalImportService>();

			try
			{
				// Ожидаем следующего запуска в 02:00 по Москве (23:00 по UTC предыдущего дня)
				await Task.Delay(
					GetDelayUntilNextRun(),
					timeProvider,
					stoppingToken);

				await importService.ImportAsync(stoppingToken);
			}
			catch (OperationCanceledException ex)
			{
				logger.LogWarning(ex, "Фоновая задача {BackgroundServiceName} прервана.", nameof(Worker));

				throw;
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Необработанная ошибка в {BackgroundServiceName}: {ExceptionMessage}", nameof(Worker), ex.Message);
			}
		}
	}

	private static TimeSpan GetDelayUntilNextRun()
	{
		var nowUtc = DateTime.UtcNow;
		var nowMoscow = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, MoscowTz);

		// Следующий запуск в 02:00 по Москве
		var nextRun = nowMoscow.Date.AddHours(2);
		if (nowMoscow >= nextRun)
			nextRun = nextRun.AddDays(1); // уже прошли — ждём завтра

		// Конвертируем обратно в UTC для корректного расчёта задержки
		var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextRun, MoscowTz);

		return nextRunUtc - nowUtc;
	}
}

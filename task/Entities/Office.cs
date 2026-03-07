using task.Enums;

namespace task.Entities;

public class Office
{
	public int Id { get; set; }

	public string? Code { get; set; }

	// Как я понял CityCode это CityId так как в файле он тоже int. Единтсвенное что, он может быть null, поэтому поменял в классе Office также на null
	public int? CityCode { get; set; }

	public string? Uuid { get; set; }

	public OfficeType? Type { get; set; }

	public string CountryCode { get; set; }

	public Coordinates Coordinates { get; set; }

	public Address Address { get; set; }

	public string WorkTime { get; set; }

	public List<Phone> Phones { get; set; } = [];

	public Office() { }
}


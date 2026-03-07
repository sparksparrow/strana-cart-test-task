namespace task.Entities;

public class Address
{
	public int Id { get; set; }

	public string? FullAddress { get; set; }

	public string? City { get; set; }

	public string? Street { get; set; }

	public string? House { get; set; }

	public string? PostalCode { get; set; }

	public int OfficeId { get; set; }

	public Office? Office { get; set; }
}

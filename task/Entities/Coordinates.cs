namespace task.Entities;

public class Coordinates
{
	public int Id { get; set; }

	public double Latitude { get; set; }

	public double Longitude { get; set; }

	public int OfficeId { get; set; }

	public Office Office { get; set; }
}

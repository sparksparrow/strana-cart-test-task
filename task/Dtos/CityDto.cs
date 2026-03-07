namespace task.Dtos;

public class CityDto
{
	public string Code { get; set; }

	public int? CityId { get; set; }

	public TerminalsDto? Terminals { get; set; }
}

namespace task.Dtos;

public class TerminalDto
{
	public string Id { get; set; }

	public string Name { get; set; }

	public string Address { get; set; }

	public string FullAddress { get; set; }

	public string Latitude { get; set; }

	public string Longitude { get; set; }

	public bool IsPvz { get; set; }

	public List<PhonesDto> Phones { get; set; }

	public bool Storage { get; set; }

	public bool IsOffice { get; set; }

	public AddressCodeDto AddressCode { get; set; }

	public string MainPhone { get; set; }

	public WorktablesDto Worktables { get; set; }
}

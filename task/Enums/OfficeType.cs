using System.ComponentModel;

namespace task.Enums;

public enum OfficeType
{
	[Description("ПВЗ")]
	PVZ = 1,
	[Description("Постамат")]
	POSTAMAT,
	[Description("Склад")]
	WAREHOUSE
}

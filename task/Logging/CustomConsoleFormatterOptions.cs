using Microsoft.Extensions.Logging.Console;

namespace task.Logging;

public sealed class CustomConsoleFormatterOptions : ConsoleFormatterOptions
{
	/// <summary>
	/// Формат времени, по умолчанию: yyyy-MM-dd HH:mm:ss
	/// </summary>
	public new string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
}

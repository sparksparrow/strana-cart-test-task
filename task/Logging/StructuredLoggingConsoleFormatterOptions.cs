using Microsoft.Extensions.Logging.Console;

namespace task.Logging;

public sealed class StructuredLoggingConsoleFormatterOptions : ConsoleFormatterOptions
{
	/// <summary>
	/// Формат времени
	/// </summary>
	public new string TimestampFormat { get; set; } = "yyyy-MM-dd HH:mm:ss";
}

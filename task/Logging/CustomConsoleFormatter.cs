using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace task.Logging;

public sealed class CustomConsoleFormatter : ConsoleFormatter, IDisposable
{
	public const string FormatterName = "StructuredLogging";

	private readonly IDisposable? _optionsMonitor;
	private CustomConsoleFormatterOptions _options;

	public CustomConsoleFormatter(IOptionsMonitor<CustomConsoleFormatterOptions> options)
		: base(FormatterName)
	{
		_optionsMonitor = options.OnChange(updated => _options = updated);
		_options = options.CurrentValue;
	}

	public override void Write<TState>(
		in LogEntry<TState> logEntry,
		IExternalScopeProvider? scopeProvider,
		TextWriter textWriter)
	{
		var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

		if (message is null && logEntry.Exception is null)
			return;

		var level = GetLevelLabel(logEntry.LogLevel);
		var time = DateTimeOffset.Now.ToString(_options.TimestampFormat ?? "yyyy-MM-dd HH:mm:ss");

		textWriter.WriteLine($"{time} {level}: {message}");

		if (logEntry.Exception is not null)
			textWriter.WriteLine($"{time} {level}: {logEntry.Exception}");
	}

	private static string GetLevelLabel(LogLevel level) => level switch
	{
		LogLevel.Trace => "TRACE",
		LogLevel.Debug => "DEBUG",
		LogLevel.Information => "INFO",
		LogLevel.Warning => "WARN",
		LogLevel.Error => "ERROR",
		LogLevel.Critical => "CRITICAL",
		_ => "NONE"
	};

	public void Dispose() => _optionsMonitor?.Dispose();
}
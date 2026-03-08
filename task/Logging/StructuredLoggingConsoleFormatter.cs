using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;

namespace task.Logging;

public sealed class StructuredLoggingConsoleFormatter : ConsoleFormatter, IDisposable
{
	public const string FormatterName = "StructuredLogging";

	private readonly IDisposable? _optionsReloadToken;
	private StructuredLoggingConsoleFormatterOptions _formatterOptions;

	public StructuredLoggingConsoleFormatter(IOptionsMonitor<StructuredLoggingConsoleFormatterOptions> options)
		: base(FormatterName) =>
		(_optionsReloadToken, _formatterOptions) =
			(options.OnChange(ReloadLoggerOptions), options.CurrentValue);

	private void ReloadLoggerOptions(StructuredLoggingConsoleFormatterOptions options) =>
		_formatterOptions = options;

	public override void Write<TState>(
		in LogEntry<TState> logEntry,
		IExternalScopeProvider? scopeProvider,
		TextWriter textWriter)
	{
		var message = logEntry.Formatter(logEntry.State, logEntry.Exception);

		if (message is null && logEntry.Exception is null)
		{
			return;
		}

		var level = GetLevelLabel(logEntry.LogLevel);
		var time = DateTimeOffset.Now.ToString(_formatterOptions.TimestampFormat ?? "yyyy-MM-dd HH:mm:ss");

		if (logEntry.Exception is null)
		{
			textWriter.WriteLine($"{time} {level}: {message}");
		}
		else
		{
			textWriter.WriteLine(
				$"{time} {level}: {message}{Environment.NewLine}{logEntry.Exception}");
		}
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

	public void Dispose() => _optionsReloadToken?.Dispose();
}
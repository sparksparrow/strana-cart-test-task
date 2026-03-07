namespace task.Configuration
{
	public class AppSettings
	{
		/// <summary>
		/// Строка подключения к PostgreSQL.
		/// </summary>
		public required string ConnectionString { get; init; }

		/// <summary>
		/// Путь к файлу terminals.json относительно корня приложения.
		/// </summary>
		public required string TerminalsFilePath { get; init; }

		/// <summary>
		/// Время запуска импорта по московскому времени (MSK).
		/// </summary>
		public required TimeOnly ImportTime { get; init; }
	}
}

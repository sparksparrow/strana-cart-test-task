using Microsoft.EntityFrameworkCore;
using task;
using task.Configuration;
using task.DbSql;
using task.Logging;
using task.Services;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddConsole(options =>
{
	options.FormatterName = CustomConsoleFormatter.FormatterName;
})
.AddConsoleFormatter<CustomConsoleFormatter, CustomConsoleFormatterOptions>();

var configuration = new ConfigurationBuilder()
	.SetBasePath(Directory.GetCurrentDirectory())
	.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
	.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
	.AddEnvironmentVariables()
	.Build();

var config = configuration.Get<AppSettings>() ?? throw new Exception($"{nameof(AppSettings)} is null");

builder.Services.AddDbContext<DellinDictionaryDbContext>(options => options.UseNpgsql(config.ConnectionString));

builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddSingleton(config);
builder.Services.AddScoped<ITerminalImportService, TerminalImportService>();

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

// Миграция БД
using (var scope = host.Services.CreateAsyncScope())
{
	var db = scope.ServiceProvider.GetRequiredService<DellinDictionaryDbContext>();
	await db.Database.MigrateAsync();
}

await host.RunAsync();

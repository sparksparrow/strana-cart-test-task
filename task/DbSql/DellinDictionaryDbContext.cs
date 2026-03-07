using Microsoft.EntityFrameworkCore;
using System.Reflection;
using task.Entities;

namespace task.DbSql;

public class DellinDictionaryDbContext(DbContextOptions<DellinDictionaryDbContext> options) : DbContext(options)
{
	public DbSet<Office> Offices { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
	}
}

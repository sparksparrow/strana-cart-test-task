using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using task.Entities;

namespace task.DbSql.Configurations;

public class CoordinatesConfiguration : IEntityTypeConfiguration<Coordinates>
{
	public void Configure(EntityTypeBuilder<Coordinates> builder)
	{
		builder.HasKey(c => c.Id);
		builder.Property(c => c.Id).ValueGeneratedOnAdd().IsRequired();
		builder.Property(c => c.Latitude).IsRequired();
		builder.Property(c => c.Longitude).IsRequired();
		builder.Property(c => c.OfficeId).IsRequired();
	}
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using task.Entities;

namespace task.DbSql.Configurations;

public class OfficeConfiguration : IEntityTypeConfiguration<Office>
{
	public void Configure(EntityTypeBuilder<Office> builder)
	{
		builder.HasKey(o => o.Id);
		builder.Property(o => o.Id).ValueGeneratedNever().IsRequired();

		builder.Property(o => o.Code)
			.HasMaxLength(50);

		builder.Property(o => o.CityCode)
			.IsRequired();

		builder.Property(o => o.Uuid)
			.HasMaxLength(50);

		builder.Property(o => o.Type)
			.HasConversion<string>()
			.HasMaxLength(50);

		builder.Property(o => o.CountryCode)
			.HasMaxLength(3)
			.IsRequired();

		builder.Property(o => o.WorkTime)
			.HasColumnType("jsonb")
			.IsRequired();

		builder.HasIndex(o => o.Code);
		builder.HasIndex(o => o.CityCode);
		builder.HasIndex(o => o.Uuid).IsUnique();

		builder.HasOne(o => o.Coordinates)
			.WithOne(c => c.Office)
			.HasForeignKey<Coordinates>(c => c.OfficeId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder.HasOne(o => o.Address)
			.WithOne(a => a.Office)
			.HasForeignKey<Address>(a => a.OfficeId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();

		builder.HasMany(o => o.Phones)
			.WithOne(p => p.Office)
			.HasForeignKey(p => p.OfficeId)
			.OnDelete(DeleteBehavior.Cascade)
			.IsRequired();
	}
}

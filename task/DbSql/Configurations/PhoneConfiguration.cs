using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using task.Entities;

namespace task.DbSql.Configurations;

public class PhoneConfiguration : IEntityTypeConfiguration<Phone>
{
	public void Configure(EntityTypeBuilder<Phone> builder)
	{
		builder.HasKey(p => p.Id);
		builder.Property(p => p.Id).ValueGeneratedOnAdd().IsRequired();
		builder.Property(p => p.PhoneNumber).HasMaxLength(20).IsRequired();
		builder.Property(p => p.OfficeId).IsRequired();
	}
}

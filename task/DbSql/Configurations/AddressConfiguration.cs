using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using task.Entities;

namespace task.DbSql.Configurations
{
	public class AddressConfiguration : IEntityTypeConfiguration<Address>
	{
		public void Configure(EntityTypeBuilder<Address> builder)
		{
			builder.HasKey(a => a.Id);
			builder.Property(a => a.Id).ValueGeneratedOnAdd().IsRequired();
			builder.Property(a => a.FullAddress).HasMaxLength(500);
			builder.Property(a => a.City).HasMaxLength(200);
			builder.Property(a => a.Street).HasMaxLength(200);
			builder.Property(a => a.House).HasMaxLength(20);
			builder.Property(a => a.PostalCode).HasMaxLength(20);
			builder.Property(a => a.OfficeId).IsRequired();
		}
	}
}

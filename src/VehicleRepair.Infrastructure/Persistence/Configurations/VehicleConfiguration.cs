using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence.Configurations;

public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
{
    public void Configure(EntityTypeBuilder<Vehicle> builder)
    {
        builder.HasKey(v => v.Id);
        builder.Property(v => v.LicensePlate).IsRequired().HasMaxLength(20);
        builder.HasIndex(v => v.LicensePlate).IsUnique();
        builder.Property(v => v.Make).IsRequired().HasMaxLength(100);
        builder.Property(v => v.Model).IsRequired().HasMaxLength(100);
        builder.Property(v => v.VIN).HasMaxLength(17);
        builder.HasIndex(v => v.VIN).IsUnique().HasFilter("\"VIN\" IS NOT NULL");
        builder.Property(v => v.VehicleType).HasConversion<int>();
        builder.Property(v => v.CreatedAt).HasDefaultValueSql("NOW()");
    }
}

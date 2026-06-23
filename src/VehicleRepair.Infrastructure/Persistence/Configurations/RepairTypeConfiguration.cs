using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence.Configurations;

public class RepairTypeConfiguration : IEntityTypeConfiguration<RepairType>
{
    public void Configure(EntityTypeBuilder<RepairType> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(1000);
        builder.HasIndex(x => x.Name).IsUnique();
    }
}

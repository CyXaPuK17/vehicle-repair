using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence.Configurations;

public class ExecutorConfiguration : IEntityTypeConfiguration<Executor>
{
    public void Configure(EntityTypeBuilder<Executor> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.INN).IsRequired().HasMaxLength(12);
        builder.HasIndex(e => e.INN).IsUnique();
        builder.Property(e => e.Name).IsRequired().HasMaxLength(500);
        builder.Property(e => e.Address).HasMaxLength(500);
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.CreatedAt).HasDefaultValueSql("NOW()");
    }
}

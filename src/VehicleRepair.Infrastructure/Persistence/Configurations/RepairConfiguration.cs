using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence.Configurations;

public class RepairConfiguration : IEntityTypeConfiguration<Repair>
{
    public void Configure(EntityTypeBuilder<Repair> builder)
    {
        builder.HasKey(r => r.Id);
        builder.Property(r => r.Cost).HasPrecision(18, 2).IsRequired();
        builder.Property(r => r.Mileage).IsRequired();
        builder.Property(r => r.Status).HasConversion<int>();
        builder.Property(r => r.Comment).HasMaxLength(1000);
        builder.Property(r => r.CreatedAt).HasDefaultValueSql("NOW()");
        builder.Property(r => r.UpdatedAt).HasDefaultValueSql("NOW()");

        builder.HasOne(r => r.Vehicle).WithMany(v => v.Repairs)
            .HasForeignKey(r => r.VehicleId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Executor).WithMany(e => e.Repairs)
            .HasForeignKey(r => r.ExecutorId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.RepairType).WithMany(rt => rt.Repairs)
            .HasForeignKey(r => r.RepairTypeId).OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.CreatedByUser).WithMany()
            .HasForeignKey(r => r.CreatedByUserId).OnDelete(DeleteBehavior.Restrict);
    }
}

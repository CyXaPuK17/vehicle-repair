using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(u => u.Id);
        builder.Property(u => u.Login).IsRequired().HasMaxLength(100);
        builder.HasIndex(u => u.Login).IsUnique();
        builder.Property(u => u.PasswordHash).IsRequired();
        builder.Property(u => u.Role).HasConversion<int>();
        builder.Property(u => u.CreatedAt).HasDefaultValueSql("NOW()");

        builder.HasOne(u => u.Customer).WithMany(c => c.Users)
            .HasForeignKey(u => u.CustomerId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(u => u.Executor).WithMany(e => e.Users)
            .HasForeignKey(u => u.ExecutorId).IsRequired(false).OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(u => u.RefreshTokens).WithOne(t => t.User)
            .HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}

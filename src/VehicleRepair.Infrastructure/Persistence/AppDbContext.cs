using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Executor> Executors => Set<Executor>();
    public DbSet<Vehicle> Vehicles => Set<Vehicle>();
    public DbSet<RepairType> RepairTypes => Set<RepairType>();
    public DbSet<Repair> Repairs => Set<Repair>();
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}

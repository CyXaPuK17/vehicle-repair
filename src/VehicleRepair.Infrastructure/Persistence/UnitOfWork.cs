using VehicleRepair.Domain.Interfaces;
using VehicleRepair.Domain.Interfaces.Repositories;
using VehicleRepair.Infrastructure.Persistence.Repositories;

namespace VehicleRepair.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _db;

    public ICustomerRepository Customers { get; }
    public IExecutorRepository Executors { get; }
    public IVehicleRepository Vehicles { get; }
    public IRepairRepository Repairs { get; }
    public IRepairTypeRepository RepairTypes { get; }
    public IUserRepository Users { get; }
    public IRefreshTokenRepository RefreshTokens { get; }

    public UnitOfWork(AppDbContext db)
    {
        _db = db;
        Customers = new CustomerRepository(db);
        Executors = new ExecutorRepository(db);
        Vehicles = new VehicleRepository(db);
        Repairs = new RepairRepository(db);
        RepairTypes = new RepairTypeRepository(db);
        Users = new UserRepository(db);
        RefreshTokens = new RefreshTokenRepository(db);
    }

    public async Task<int> SaveChangesAsync(CancellationToken ct = default) =>
        await _db.SaveChangesAsync(ct);
}

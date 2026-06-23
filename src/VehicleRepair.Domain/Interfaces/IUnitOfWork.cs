using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Domain.Interfaces;

public interface IUnitOfWork
{
    ICustomerRepository Customers { get; }
    IExecutorRepository Executors { get; }
    IVehicleRepository Vehicles { get; }
    IRepairRepository Repairs { get; }
    IRepairTypeRepository RepairTypes { get; }
    IUserRepository Users { get; }
    IRefreshTokenRepository RefreshTokens { get; }

    Task<int> SaveChangesAsync(CancellationToken ct = default);
}

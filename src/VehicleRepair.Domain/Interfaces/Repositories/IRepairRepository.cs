using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Domain.Interfaces.Repositories;

public interface IRepairRepository : IRepository<Repair>
{
    Task<IReadOnlyList<Repair>> GetByExecutorIdAsync(Guid executorId, DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetByVehicleIdAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetByVehicleIdsAsync(IEnumerable<Guid> vehicleIds, DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetByCustomerIdAsync(Guid customerId, DateTime from, DateTime to, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<Repair?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetAllActiveAsync(CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<IReadOnlyList<Repair>> GetAllByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default);
}

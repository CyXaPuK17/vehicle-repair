using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Domain.Interfaces.Repositories;

public interface IRepairTypeRepository : IRepository<RepairType>
{
    Task<IReadOnlyList<RepairType>> GetActiveAsync(CancellationToken ct = default);
    Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
}

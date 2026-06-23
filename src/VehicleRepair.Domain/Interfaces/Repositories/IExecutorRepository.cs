using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Domain.Interfaces.Repositories;

public interface IExecutorRepository : IRepository<Executor>
{
    Task<Executor?> GetByINNAsync(string inn, CancellationToken ct = default);
    Task<bool> ExistsByINNAsync(string inn, CancellationToken ct = default);
    Task<IReadOnlyList<Executor>> GetActiveAsync(CancellationToken ct = default);
}

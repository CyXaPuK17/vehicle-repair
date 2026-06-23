using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class ExecutorRepository : Repository<Executor>, IExecutorRepository
{
    public ExecutorRepository(AppDbContext db) : base(db) { }

    public async Task<Executor?> GetByINNAsync(string inn, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(e => e.INN == inn, ct);

    public async Task<bool> ExistsByINNAsync(string inn, CancellationToken ct = default) =>
        await _set.AnyAsync(e => e.INN == inn, ct);

    public async Task<IReadOnlyList<Executor>> GetActiveAsync(CancellationToken ct = default) =>
        await _set.Where(e => e.IsActive).ToListAsync(ct);
}

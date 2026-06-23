using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class RepairTypeRepository : Repository<RepairType>, IRepairTypeRepository
{
    public RepairTypeRepository(AppDbContext db) : base(db) { }

    public async Task<IReadOnlyList<RepairType>> GetActiveAsync(CancellationToken ct = default) =>
        await _set.Where(rt => rt.IsActive).OrderBy(rt => rt.Name).ToListAsync(ct);

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default) =>
        await _set.AnyAsync(rt => rt.Name == name, ct);
}

using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class CustomerRepository : Repository<Customer>, ICustomerRepository
{
    public CustomerRepository(AppDbContext db) : base(db) { }

    public async Task<Customer?> GetByINNAsync(string inn, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(c => c.INN == inn, ct);

    public async Task<bool> ExistsByINNAsync(string inn, CancellationToken ct = default) =>
        await _set.AnyAsync(c => c.INN == inn, ct);

    public async Task<IReadOnlyList<Customer>> GetActiveAsync(CancellationToken ct = default) =>
        await _set.Where(c => c.IsActive).ToListAsync(ct);
}

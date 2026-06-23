using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(AppDbContext db) : base(db) { }

    public async Task<User?> GetByLoginAsync(string login, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(u => u.Login == login, ct);

    public async Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default) =>
        await _set.AnyAsync(u => u.Login == login, ct);

    public async Task<User?> GetWithTokensAsync(Guid id, CancellationToken ct = default) =>
        await _set.Include(u => u.RefreshTokens).FirstOrDefaultAsync(u => u.Id == id, ct);

    public override async Task<IReadOnlyList<User>> GetAllAsync(CancellationToken ct = default) =>
        await _set.Include(u => u.Customer).Include(u => u.Executor).ToListAsync(ct);

    public async Task<User?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await _set.Include(u => u.Customer).Include(u => u.Executor)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
}

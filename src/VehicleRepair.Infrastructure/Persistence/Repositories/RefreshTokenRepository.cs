using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : Repository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext db) : base(db) { }

    public async Task<RefreshToken?> GetActiveByTokenAsync(string token, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked, ct);

    public async Task RevokeAllForUserAsync(Guid userId, CancellationToken ct = default)
    {
        var tokens = await _set.Where(t => t.UserId == userId && !t.IsRevoked).ToListAsync(ct);
        foreach (var t in tokens) t.IsRevoked = true;
    }
}

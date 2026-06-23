using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Domain.Interfaces.Repositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginAsync(string login, CancellationToken ct = default);
    Task<bool> ExistsByLoginAsync(string login, CancellationToken ct = default);
    Task<User?> GetWithTokensAsync(Guid id, CancellationToken ct = default);
    Task<User?> GetWithDetailsAsync(Guid id, CancellationToken ct = default);
}

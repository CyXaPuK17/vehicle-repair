using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.GetAll;

public class GetAllUsersUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetAllUsersUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<UserDto>> ExecuteAsync(CancellationToken ct)
    {
        var users = await _uow.Users.GetAllAsync(ct);
        return users
            // Собственную учётную запись скрываем — иначе УК может деактивировать сама себя.
            .Where(u => u.Id != _currentUser.UserId)
            .Select(u => new UserDto(
            u.Id,
            u.Login,
            u.Role,
            u.CustomerId,
            u.ExecutorId,
            u.Customer?.Name ?? u.Executor?.Name,
            u.IsActive,
            u.LastLoginAt,
            u.CreatedAt
        )).ToList();
    }
}

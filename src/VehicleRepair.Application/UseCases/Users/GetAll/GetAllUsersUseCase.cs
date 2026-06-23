using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.GetAll;

public class GetAllUsersUseCase
{
    private readonly IUnitOfWork _uow;

    public GetAllUsersUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<UserDto>> ExecuteAsync(CancellationToken ct)
    {
        var users = await _uow.Users.GetAllAsync(ct);
        return users.Select(u => new UserDto(
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

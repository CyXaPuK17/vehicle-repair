using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.GetCurrent;

public class GetCurrentUserUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetCurrentUserUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<UserDto> ExecuteAsync(CancellationToken ct)
    {
        var user = await _uow.Users.GetWithDetailsAsync(_currentUser.UserId, ct)
            ?? throw new NotFoundException("User", _currentUser.UserId);
        return new UserDto(
            user.Id, user.Login, user.Role,
            user.CustomerId, user.ExecutorId,
            user.Customer?.Name ?? user.Executor?.Name,
            user.IsActive, user.LastLoginAt, user.CreatedAt
        );
    }
}

using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.GetProfile;

public class GetMyProfileUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetMyProfileUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ProfileDto> ExecuteAsync(CancellationToken ct)
    {
        var user = await _uow.Users.GetWithDetailsAsync(_currentUser.UserId, ct)
            ?? throw new NotFoundException("User", _currentUser.UserId);

        var customer = user.Customer;
        var executor = user.Executor;

        return new ProfileDto(
            user.Login,
            user.Role,
            customer?.Name ?? executor?.Name,
            customer?.INN ?? executor?.INN,
            customer?.ContactPerson,
            executor?.Address,
            customer?.Phone ?? executor?.Phone,
            customer?.Email ?? executor?.Email,
            user.LastLoginAt,
            user.CreatedAt
        );
    }
}

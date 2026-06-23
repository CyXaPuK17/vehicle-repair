using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.ChangePassword;

public class ChangePasswordUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;

    public ChangePasswordUseCase(IUnitOfWork uow, IPasswordHasher hasher)
    {
        _uow = uow;
        _hasher = hasher;
    }

    public async Task ExecuteAsync(Guid userId, ChangePasswordRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException(nameof(User), userId);

        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
            throw new DomainException("Текущий пароль неверен.");

        user.PasswordHash = _hasher.Hash(request.NewPassword);
        _uow.Users.Update(user);
        await _uow.SaveChangesAsync(ct);
    }
}

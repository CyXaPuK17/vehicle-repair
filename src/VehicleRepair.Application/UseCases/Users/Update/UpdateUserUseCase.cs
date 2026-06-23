using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.Update;

public class UpdateUserUseCase
{
    private readonly IUnitOfWork _uow;

    public UpdateUserUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid userId, UpdateUserRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);

        if (user.Login != request.Login && await _uow.Users.ExistsByLoginAsync(request.Login, ct))
            throw new DomainException($"Пользователь с логином '{request.Login}' уже существует.");

        user.Login = request.Login;
        user.Role = request.Role;
        user.CustomerId = request.CustomerId;
        user.ExecutorId = request.ExecutorId;

        await _uow.SaveChangesAsync(ct);
    }
}

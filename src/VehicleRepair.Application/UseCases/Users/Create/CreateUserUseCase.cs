using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.Create;

public class CreateUserUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;

    public CreateUserUseCase(IUnitOfWork uow, IPasswordHasher hasher)
    {
        _uow = uow;
        _hasher = hasher;
    }

    public async Task<Guid> ExecuteAsync(CreateUserRequest request, CancellationToken ct)
    {
        if (await _uow.Users.ExistsByLoginAsync(request.Login, ct))
            throw new DomainException($"Пользователь с логином '{request.Login}' уже существует.");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = request.Login,
            PasswordHash = _hasher.Hash(request.Password),
            Role = request.Role,
            CustomerId = request.CustomerId,
            ExecutorId = request.ExecutorId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);
        return user.Id;
    }
}

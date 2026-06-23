using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.SetActive;

public class SetUserActiveUseCase
{
    private readonly IUnitOfWork _uow;

    public SetUserActiveUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid userId, bool isActive, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);

        user.IsActive = isActive;
        await _uow.SaveChangesAsync(ct);
    }
}

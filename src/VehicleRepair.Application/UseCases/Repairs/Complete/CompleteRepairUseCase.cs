using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.Complete;

public class CompleteRepairUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CompleteRepairUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid id, CancellationToken ct)
    {
        var repair = await _uow.Repairs.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Repair), id);

        if (repair.ExecutorId != _currentUser.LinkedEntityId)
            throw new ForbiddenException("Нет доступа к этому ремонту.");

        if (repair.Status != RepairStatus.InProgress)
            throw new DomainException("Завершить можно только ремонт, находящийся в работе.");

        repair.Status = RepairStatus.Completed;
        repair.UpdatedAt = DateTime.UtcNow;

        _uow.Repairs.Update(repair);
        await _uow.SaveChangesAsync(ct);
    }
}

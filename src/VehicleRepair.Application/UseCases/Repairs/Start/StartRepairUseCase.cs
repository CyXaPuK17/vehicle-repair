using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.Start;

public class StartRepairUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public StartRepairUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
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

        if (repair.Status != RepairStatus.Received)
            throw new DomainException("В работу можно взять только принятый ремонт.");

        repair.Status = RepairStatus.InProgress;
        repair.UpdatedAt = DateTime.UtcNow;

        _uow.Repairs.Update(repair);
        await _uow.SaveChangesAsync(ct);
    }
}

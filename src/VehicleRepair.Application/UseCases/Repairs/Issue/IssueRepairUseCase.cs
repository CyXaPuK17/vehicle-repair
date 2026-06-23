using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.Issue;

public class IssueRepairUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public IssueRepairUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid id, IssueRepairRequest request, CancellationToken ct)
    {
        var repair = await _uow.Repairs.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Repair), id);

        if (repair.Status == RepairStatus.Issued)
            throw new DomainException("Ремонт уже выдан.");

        if (_currentUser.Role == UserRole.Executor && repair.ExecutorId != _currentUser.LinkedEntityId)
            throw new ForbiddenException("Нет доступа к этому ремонту.");

        if (request.IssuedAt < repair.ReceivedAt)
            throw new DomainException("Дата выдачи не может быть раньше даты приёмки.");

        repair.IssuedAt = request.IssuedAt;
        repair.Status = RepairStatus.Issued;
        repair.UpdatedAt = DateTime.UtcNow;

        _uow.Repairs.Update(repair);
        await _uow.SaveChangesAsync(ct);
    }
}

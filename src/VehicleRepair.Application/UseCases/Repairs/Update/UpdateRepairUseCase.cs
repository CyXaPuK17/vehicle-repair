using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.Update;

public class UpdateRepairUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateRepairUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(Guid id, UpdateRepairRequest request, CancellationToken ct)
    {
        var repair = await _uow.Repairs.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Repair), id);

        if (repair.Status == RepairStatus.Issued)
            throw new DomainException("Нельзя редактировать выданный ремонт.");

        if (request.Mileage < 0)
            throw new DomainException("Пробег не может быть отрицательным.");

        if (_currentUser.Role == UserRole.Executor && repair.ExecutorId != _currentUser.LinkedEntityId)
            throw new ForbiddenException("Нет доступа к этому ремонту.");

        if (await _uow.RepairTypes.GetByIdAsync(request.RepairTypeId, ct) is null)
            throw new NotFoundException(nameof(RepairType), request.RepairTypeId);

        repair.RepairTypeId = request.RepairTypeId;
        repair.ReceivedAt = request.ReceivedAt;
        repair.Cost = request.Cost;
        repair.Mileage = request.Mileage;
        repair.Comment = request.Comment;
        repair.UpdatedAt = DateTime.UtcNow;

        _uow.Repairs.Update(repair);
        await _uow.SaveChangesAsync(ct);
    }
}

using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.Create;

public class CreateRepairUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public CreateRepairUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<Guid> ExecuteAsync(CreateRepairRequest request, CancellationToken ct)
    {
        var vehicle = await _uow.Vehicles.GetByIdAsync(request.VehicleId, ct)
            ?? throw new NotFoundException(nameof(Vehicle), request.VehicleId);

        if (await _uow.RepairTypes.GetByIdAsync(request.RepairTypeId, ct) is null)
            throw new NotFoundException(nameof(RepairType), request.RepairTypeId);

        // Determine executorId: ManagementCompany must provide it separately — here executor = current user's linked entity
        var executorId = _currentUser.Role == UserRole.Executor
            ? _currentUser.LinkedEntityId ?? throw new ForbiddenException("Исполнитель не привязан к аккаунту.")
            : throw new ForbiddenException("Создавать ремонты могут только Исполнители.");

        if (request.Mileage < 0)
            throw new DomainException("Пробег не может быть отрицательным.");

        var repair = new Repair
        {
            Id = Guid.NewGuid(),
            VehicleId = request.VehicleId,
            ExecutorId = executorId,
            RepairTypeId = request.RepairTypeId,
            ReceivedAt = request.ReceivedAt,
            Cost = request.Cost,
            Mileage = request.Mileage,
            Comment = request.Comment,
            Status = RepairStatus.Received,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedByUserId = _currentUser.UserId
        };

        await _uow.Repairs.AddAsync(repair, ct);
        await _uow.SaveChangesAsync(ct);
        return repair.Id;
    }
}

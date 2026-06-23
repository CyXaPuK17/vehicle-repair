using VehicleRepair.Application.DTOs.RepairTypes;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.RepairTypes.Update;

public class UpdateRepairTypeUseCase
{
    private readonly IUnitOfWork _uow;

    public UpdateRepairTypeUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid id, UpdateRepairTypeRequest request, CancellationToken ct)
    {
        var repairType = await _uow.RepairTypes.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(RepairType), id);

        repairType.Name = request.Name;
        repairType.Description = request.Description;
        repairType.IsActive = request.IsActive;

        await _uow.SaveChangesAsync(ct);
    }
}

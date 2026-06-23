using VehicleRepair.Application.DTOs.RepairTypes;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.RepairTypes.GetAll;

public class GetAllRepairTypesUseCase
{
    private readonly IUnitOfWork _uow;

    public GetAllRepairTypesUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<RepairTypeDto>> ExecuteAsync(CancellationToken ct)
    {
        var types = await _uow.RepairTypes.GetActiveAsync(ct);
        return types.Select(t => new RepairTypeDto(t.Id, t.Name, t.Description, t.IsActive)).ToList();
    }
}

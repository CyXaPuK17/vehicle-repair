using VehicleRepair.Application.DTOs.RepairTypes;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.RepairTypes.Create;

public class CreateRepairTypeUseCase
{
    private readonly IUnitOfWork _uow;

    public CreateRepairTypeUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> ExecuteAsync(CreateRepairTypeRequest request, CancellationToken ct)
    {
        if (await _uow.RepairTypes.ExistsByNameAsync(request.Name, ct))
            throw new DomainException($"Вид ремонта '{request.Name}' уже существует.");

        var type = new RepairType
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description
        };

        await _uow.RepairTypes.AddAsync(type, ct);
        await _uow.SaveChangesAsync(ct);
        return type.Id;
    }
}

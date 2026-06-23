using VehicleRepair.Application.DTOs.Executors;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Executors.GetAll;

public class GetAllExecutorsUseCase
{
    private readonly IUnitOfWork _uow;

    public GetAllExecutorsUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<ExecutorDto>> ExecuteAsync(CancellationToken ct)
    {
        var executors = await _uow.Executors.GetAllAsync(ct);
        return executors.Select(e => new ExecutorDto(
            e.Id, e.INN, e.Name, e.Address, e.Phone, e.Email, e.IsActive, e.CreatedAt
        )).ToList();
    }
}

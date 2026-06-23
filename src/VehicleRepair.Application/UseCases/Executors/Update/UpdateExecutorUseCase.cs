using VehicleRepair.Application.DTOs.Executors;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Executors.Update;

public class UpdateExecutorUseCase
{
    private readonly IUnitOfWork _uow;

    public UpdateExecutorUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid id, UpdateExecutorRequest request, CancellationToken ct)
    {
        var executor = await _uow.Executors.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Executor), id);

        executor.Name = request.Name;
        executor.Address = request.Address;
        executor.Phone = request.Phone;
        executor.Email = request.Email;
        executor.IsActive = request.IsActive;

        _uow.Executors.Update(executor);
        await _uow.SaveChangesAsync(ct);
    }
}

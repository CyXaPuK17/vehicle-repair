using VehicleRepair.Application.DTOs.Executors;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Executors.Create;

public class CreateExecutorUseCase
{
    private readonly IUnitOfWork _uow;

    public CreateExecutorUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> ExecuteAsync(CreateExecutorRequest request, CancellationToken ct)
    {
        if (await _uow.Executors.ExistsByINNAsync(request.INN, ct))
            throw new DomainException($"Исполнитель с ИНН '{request.INN}' уже существует.");

        var executor = new Executor
        {
            Id = Guid.NewGuid(),
            INN = request.INN,
            Name = request.Name,
            Address = request.Address,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Executors.AddAsync(executor, ct);
        await _uow.SaveChangesAsync(ct);
        return executor.Id;
    }
}

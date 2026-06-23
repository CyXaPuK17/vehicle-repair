using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Customers.Create;

public class CreateCustomerUseCase
{
    private readonly IUnitOfWork _uow;

    public CreateCustomerUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> ExecuteAsync(CreateCustomerRequest request, CancellationToken ct)
    {
        if (await _uow.Customers.ExistsByINNAsync(request.INN, ct))
            throw new DomainException($"Заказчик с ИНН '{request.INN}' уже существует.");

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            INN = request.INN,
            Name = request.Name,
            ContactPerson = request.ContactPerson,
            Phone = request.Phone,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Customers.AddAsync(customer, ct);
        await _uow.SaveChangesAsync(ct);
        return customer.Id;
    }
}

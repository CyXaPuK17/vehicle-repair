using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Customers.GetById;

public class GetCustomerByIdUseCase
{
    private readonly IUnitOfWork _uow;

    public GetCustomerByIdUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<CustomerDto> ExecuteAsync(Guid id, CancellationToken ct)
    {
        var c = await _uow.Customers.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Customer), id);
        return new CustomerDto(c.Id, c.INN, c.Name, c.ContactPerson, c.Phone, c.Email, c.IsActive, c.CreatedAt);
    }
}

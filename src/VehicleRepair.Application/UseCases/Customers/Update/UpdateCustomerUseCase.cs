using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Customers.Update;

public class UpdateCustomerUseCase
{
    private readonly IUnitOfWork _uow;

    public UpdateCustomerUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid id, UpdateCustomerRequest request, CancellationToken ct)
    {
        var customer = await _uow.Customers.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Customer), id);

        customer.Name = request.Name;
        customer.ContactPerson = request.ContactPerson;
        customer.Phone = request.Phone;
        customer.Email = request.Email;
        customer.IsActive = request.IsActive;

        _uow.Customers.Update(customer);
        await _uow.SaveChangesAsync(ct);
    }
}

using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Customers.GetAll;

public class GetAllCustomersUseCase
{
    private readonly IUnitOfWork _uow;

    public GetAllCustomersUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<IReadOnlyList<CustomerDto>> ExecuteAsync(CancellationToken ct)
    {
        var customers = await _uow.Customers.GetAllAsync(ct);
        return customers.Select(c => new CustomerDto(
            c.Id, c.INN, c.Name, c.ContactPerson, c.Phone, c.Email, c.IsActive, c.CreatedAt
        )).ToList();
    }
}

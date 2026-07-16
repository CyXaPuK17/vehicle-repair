using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.SetActive;

public class SetUserActiveUseCase
{
    private readonly IUnitOfWork _uow;

    public SetUserActiveUseCase(IUnitOfWork uow) => _uow = uow;

    // Активность заказчика/исполнителя/ТС — производная от активности привязанного пользователя,
    // отдельного переключателя для них в интерфейсе больше нет.
    public async Task ExecuteAsync(Guid userId, bool isActive, CancellationToken ct)
    {
        var user = await _uow.Users.GetByIdAsync(userId, ct)
            ?? throw new NotFoundException("User", userId);

        user.IsActive = isActive;

        if (user.CustomerId.HasValue)
        {
            var customer = await _uow.Customers.GetByIdAsync(user.CustomerId.Value, ct);
            if (customer is not null)
            {
                customer.IsActive = isActive;
                foreach (var vehicle in await _uow.Vehicles.GetByCustomerIdAsync(customer.Id, ct))
                    vehicle.IsActive = isActive;
            }
        }
        else if (user.ExecutorId.HasValue)
        {
            var executor = await _uow.Executors.GetByIdAsync(user.ExecutorId.Value, ct);
            if (executor is not null)
                executor.IsActive = isActive;
        }

        await _uow.SaveChangesAsync(ct);
    }
}

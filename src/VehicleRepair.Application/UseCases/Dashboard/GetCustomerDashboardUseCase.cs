using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Dashboard;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Dashboard;

public class GetCustomerDashboardUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerDashboardUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CustomerDashboardDto> ExecuteAsync(CancellationToken ct)
    {
        var customerId = _currentUser.LinkedEntityId
            ?? throw new ForbiddenException("Профиль клиента не привязан к учётной записи.");

        var now = DateTime.UtcNow;
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vehiclesTask     = _uow.Vehicles.GetByCustomerIdAsync(customerId, ct);
        var activeTask       = _uow.Repairs.GetActiveByCustomerIdAsync(customerId, ct);
        var yearRepairsTask  = _uow.Repairs.GetByCustomerIdAsync(customerId, yearStart, now, ct);

        await Task.WhenAll(vehiclesTask, activeTask, yearRepairsTask);

        var vehicles     = vehiclesTask.Result;
        var activeRepairs = activeTask.Result;
        var yearRepairs  = yearRepairsTask.Result;

        return new CustomerDashboardDto(
            vehicles.Count,
            activeRepairs.Count,
            yearRepairs.Sum(r => r.Cost)
        );
    }
}

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

        var vehicles = await _uow.Vehicles.GetByCustomerIdAsync(customerId, ct);

        var activeRepairs = await _uow.Repairs.GetActiveByCustomerIdAsync(customerId, ct);

        var now = DateTime.UtcNow;
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var yearRepairs = await _uow.Repairs.GetByCustomerIdAsync(customerId, yearStart, now, ct);

        return new CustomerDashboardDto(
            vehicles.Count,
            activeRepairs.Count,
            yearRepairs.Sum(r => r.Cost)
        );
    }
}

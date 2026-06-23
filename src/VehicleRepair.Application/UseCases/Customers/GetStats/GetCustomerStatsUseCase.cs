using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Customers.GetStats;

public class GetCustomerStatsUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerStatsUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<CustomerStatsDto> ExecuteAsync(CancellationToken ct)
    {
        var customerId = _currentUser.LinkedEntityId
            ?? throw new UnauthorizedAccessException();

        var now       = DateTime.UtcNow;
        var yearStart = new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vehicles      = await _uow.Vehicles.FindAsync(v => v.CustomerId == customerId && v.IsActive, ct);
        var activeRepairs = await _uow.Repairs.GetActiveByCustomerIdAsync(customerId, ct);
        var yearRepairs   = await _uow.Repairs.GetByCustomerIdAsync(customerId, yearStart, now, ct);

        return new CustomerStatsDto(
            vehicles.Count,
            activeRepairs.Count,
            yearRepairs.Count,
            yearRepairs.Sum(r => r.Cost)
        );
    }
}

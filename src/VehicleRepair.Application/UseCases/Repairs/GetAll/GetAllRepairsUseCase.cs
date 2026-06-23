using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Repairs.GetAll;

public class GetAllRepairsUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetAllRepairsUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<PagedResult<RepairDto>> ExecuteAsync(
        DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct)
    {
        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 200) pageSize = 50;

        IReadOnlyList<Domain.Entities.Repair> repairs;

        if (_currentUser.Role == UserRole.Customer && _currentUser.LinkedEntityId.HasValue)
        {
            repairs = await _uow.Repairs.GetActiveByCustomerIdAsync(_currentUser.LinkedEntityId.Value, ct);
        }
        else
        {
            var dateFrom = from ?? new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var dateTo = to ?? new DateTime(2100, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            repairs = _currentUser.Role == UserRole.Executor && _currentUser.LinkedEntityId.HasValue
                ? await _uow.Repairs.GetByExecutorIdAsync(_currentUser.LinkedEntityId.Value, dateFrom, dateTo, ct)
                : await _uow.Repairs.GetByPeriodAsync(dateFrom, dateTo, ct);
        }

        var total = repairs.Count;
        var items = repairs.Skip((page - 1) * pageSize).Take(pageSize).Select(MapToDto).ToList();

        return new PagedResult<RepairDto> { Items = items, Page = page, PageSize = pageSize, Total = total };
    }

    internal static RepairDto MapToDto(Domain.Entities.Repair r) => new(
        r.Id,
        r.VehicleId,
        r.Vehicle?.LicensePlate ?? string.Empty,
        r.Vehicle != null ? $"{r.Vehicle.Make} {r.Vehicle.Model}" : string.Empty,
        r.Vehicle?.CustomerId ?? Guid.Empty,
        r.Vehicle?.Customer?.Name ?? string.Empty,
        r.ExecutorId,
        r.Executor?.Name ?? string.Empty,
        r.RepairTypeId,
        r.RepairType?.Name ?? string.Empty,
        r.ReceivedAt,
        r.IssuedAt,
        r.Cost,
        r.Mileage,
        r.Status,
        r.Comment,
        r.CreatedAt
    );
}

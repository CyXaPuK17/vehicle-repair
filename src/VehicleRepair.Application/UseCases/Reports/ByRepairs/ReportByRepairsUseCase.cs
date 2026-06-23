using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Reports;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Reports.ByRepairs;

public class ReportByRepairsUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ReportByRepairsUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ReportByRepairsDto> ExecuteAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        var repairs = _currentUser.Role == UserRole.Executor && _currentUser.LinkedEntityId.HasValue
            ? await _uow.Repairs.GetByExecutorIdAsync(_currentUser.LinkedEntityId.Value, from, to, ct)
            : await _uow.Repairs.GetByPeriodAsync(from, to, ct);

        var rows = repairs.Select(r => new ReportByRepairsRow(
            r.Id,
            r.Vehicle?.LicensePlate ?? string.Empty,
            r.Vehicle != null ? $"{r.Vehicle.Make} {r.Vehicle.Model}" : string.Empty,
            r.Vehicle?.Customer?.Name ?? string.Empty,
            r.Executor?.Name ?? string.Empty,
            r.RepairType?.Name ?? string.Empty,
            r.ReceivedAt,
            r.IssuedAt,
            r.Mileage,
            r.Cost
        )).ToList();

        return new ReportByRepairsDto(rows, rows.Count, rows.Sum(r => r.Cost));
    }
}

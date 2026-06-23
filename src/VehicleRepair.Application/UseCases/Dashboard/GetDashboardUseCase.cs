using VehicleRepair.Application.DTOs.Dashboard;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Dashboard;

public class GetDashboardUseCase
{
    private readonly IUnitOfWork _uow;

    public GetDashboardUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<DashboardDto> ExecuteAsync(
        DateTime from, DateTime to, int topCount, CancellationToken ct)
    {
        var active = await _uow.Repairs.GetAllActiveAsync(ct);
        var period = await _uow.Repairs.GetByPeriodAsync(from, to, ct);

        var top = period
            .GroupBy(r => r.Executor?.Name ?? "—")
            .Select(g => new ExecutorStatDto(g.Key, g.Count(), g.Sum(r => r.Cost)))
            .OrderByDescending(e => e.Count)
            .Take(topCount)
            .ToList();

        return new DashboardDto(
            active.Count(r => r.Status == RepairStatus.Received),
            active.Count(r => r.Status == RepairStatus.InProgress),
            active.Count(r => r.Status == RepairStatus.Completed),
            period.Count,
            period.Sum(r => r.Cost),
            top
        );
    }
}

using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Executors;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Executors.GetStats;

public class GetExecutorStatsUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetExecutorStatsUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ExecutorStatsDto> ExecuteAsync(CancellationToken ct)
    {
        var executorId = _currentUser.LinkedEntityId
            ?? throw new UnauthorizedAccessException();

        var now        = DateTime.UtcNow;
        var yearStart  = new DateTime(now.Year, 1,        1, 0, 0, 0, DateTimeKind.Utc);
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var allTime    = new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var allRepairs  = await _uow.Repairs.GetByExecutorIdAsync(executorId, allTime, now, ct);
        var yearRepairs = allRepairs.Where(r => r.ReceivedAt >= yearStart).ToList();
        var monthRepairs = yearRepairs.Where(r => r.ReceivedAt >= monthStart).ToList();

        var done = new HashSet<RepairStatus> { RepairStatus.Completed, RepairStatus.Issued };

        return new ExecutorStatsDto(
            allRepairs.Count(r => r.Status != RepairStatus.Issued),
            monthRepairs.Count(r => done.Contains(r.Status)),
            yearRepairs.Count(r => done.Contains(r.Status)),
            monthRepairs.Sum(r => r.Cost),
            yearRepairs.Sum(r => r.Cost)
        );
    }
}

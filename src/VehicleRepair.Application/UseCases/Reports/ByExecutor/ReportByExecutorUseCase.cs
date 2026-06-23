using VehicleRepair.Application.DTOs.Reports;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Reports.ByExecutor;

public class ReportByExecutorUseCase
{
    private readonly IUnitOfWork _uow;

    public ReportByExecutorUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<ReportByExecutorDto> ExecuteAsync(
        Guid? executorId, DateTime from, DateTime to, CancellationToken ct)
    {
        var executors = executorId.HasValue
            ? await _uow.Executors.FindAsync(e => e.Id == executorId.Value, ct)
            : await _uow.Executors.GetActiveAsync(ct);

        var rows = new List<ReportByExecutorRow>();
        foreach (var executor in executors)
        {
            var repairs = await _uow.Repairs.GetByExecutorIdAsync(executor.Id, from, to, ct);
            rows.Add(new ReportByExecutorRow(
                executor.Id,
                executor.Name,
                executor.INN,
                repairs.Count,
                repairs.Sum(r => r.Cost)
            ));
        }

        return new ReportByExecutorDto(rows, rows.Sum(r => r.RepairCount), rows.Sum(r => r.TotalCost));
    }
}

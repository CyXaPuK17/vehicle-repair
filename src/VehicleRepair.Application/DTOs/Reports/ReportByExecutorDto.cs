namespace VehicleRepair.Application.DTOs.Reports;

public record ReportByExecutorDto(
    IReadOnlyList<ReportByExecutorRow> Rows,
    int TotalCount,
    decimal TotalCost
);

public record ReportByExecutorRow(
    Guid ExecutorId,
    string ExecutorName,
    string ExecutorINN,
    int RepairCount,
    decimal TotalCost
);

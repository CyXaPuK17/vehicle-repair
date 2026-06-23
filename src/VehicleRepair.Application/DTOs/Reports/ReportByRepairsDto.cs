namespace VehicleRepair.Application.DTOs.Reports;

public record ReportByRepairsDto(
    IReadOnlyList<ReportByRepairsRow> Rows,
    int TotalCount,
    decimal TotalCost
);

public record ReportByRepairsRow(
    Guid RepairId,
    string LicensePlate,
    string VehicleMakeModel,
    string CustomerName,
    string ExecutorName,
    string RepairTypeName,
    DateTime ReceivedAt,
    DateTime? IssuedAt,
    int Mileage,
    decimal Cost
);

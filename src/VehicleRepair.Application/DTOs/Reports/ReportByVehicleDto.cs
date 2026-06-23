namespace VehicleRepair.Application.DTOs.Reports;

public record ReportByVehicleDto(
    IReadOnlyList<ReportByVehicleRow> Rows
);

public record ReportByVehicleRow(
    Guid VehicleId,
    string LicensePlate,
    string MakeModel,
    string CustomerName,
    decimal TotalCost,
    int MileageDelta,
    IReadOnlyList<ReportByVehicleRepairRow> Repairs
);

public record ReportByVehicleRepairRow(
    string RepairTypeName,
    string ExecutorName,
    DateTime ReceivedAt,
    int Mileage,
    decimal Cost
);

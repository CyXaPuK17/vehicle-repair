namespace VehicleRepair.Application.DTOs.Reports;

public record ReportByCustomerDto(
    IReadOnlyList<ReportByCustomerRow> Rows,
    int TotalCount,
    decimal TotalCost
);

public record ReportByCustomerRow(
    Guid CustomerId,
    string CustomerName,
    string CustomerINN,
    int RepairCount,
    decimal TotalCost
);

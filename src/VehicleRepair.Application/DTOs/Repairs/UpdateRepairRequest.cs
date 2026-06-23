namespace VehicleRepair.Application.DTOs.Repairs;

public record UpdateRepairRequest(
    Guid RepairTypeId,
    DateTime ReceivedAt,
    decimal Cost,
    int Mileage,
    string? Comment
);

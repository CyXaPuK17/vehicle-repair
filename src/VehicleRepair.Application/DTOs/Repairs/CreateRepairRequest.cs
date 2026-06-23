namespace VehicleRepair.Application.DTOs.Repairs;

public record CreateRepairRequest(
    Guid VehicleId,
    Guid RepairTypeId,
    DateTime ReceivedAt,
    decimal Cost,
    int Mileage,
    string? Comment
);

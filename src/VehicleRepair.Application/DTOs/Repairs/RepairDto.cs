using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Repairs;

public record RepairDto(
    Guid Id,
    Guid VehicleId,
    string LicensePlate,
    string VehicleMakeModel,
    Guid CustomerId,
    string CustomerName,
    Guid ExecutorId,
    string ExecutorName,
    Guid RepairTypeId,
    string RepairTypeName,
    DateTime ReceivedAt,
    DateTime? IssuedAt,
    decimal Cost,
    int Mileage,
    RepairStatus Status,
    string? Comment,
    DateTime CreatedAt
);

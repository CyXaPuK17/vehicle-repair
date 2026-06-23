using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Vehicles;

public record VehicleDto(
    Guid Id,
    string LicensePlate,
    string Make,
    string Model,
    int? Year,
    string? VIN,
    VehicleType VehicleType,
    bool IsActive,
    Guid CustomerId,
    string CustomerName
);

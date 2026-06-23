namespace VehicleRepair.Application.DTOs.RepairTypes;

public record RepairTypeDto(Guid Id, string Name, string? Description, bool IsActive);

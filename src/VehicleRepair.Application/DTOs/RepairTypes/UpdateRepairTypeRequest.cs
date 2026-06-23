namespace VehicleRepair.Application.DTOs.RepairTypes;

public record UpdateRepairTypeRequest(string Name, string? Description, bool IsActive);

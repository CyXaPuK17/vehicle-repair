namespace VehicleRepair.Application.DTOs.Executors;

public record UpdateExecutorRequest(
    string Name,
    string? Address,
    string? Phone,
    string? Email,
    bool IsActive
);

namespace VehicleRepair.Application.DTOs.Executors;

public record CreateExecutorRequest(
    string INN,
    string Name,
    string? Address,
    string? Phone,
    string? Email
);

namespace VehicleRepair.Application.DTOs.Executors;

public record ExecutorDto(
    Guid Id,
    string INN,
    string Name,
    string? Address,
    string? Phone,
    string? Email,
    bool IsActive,
    DateTime CreatedAt
);

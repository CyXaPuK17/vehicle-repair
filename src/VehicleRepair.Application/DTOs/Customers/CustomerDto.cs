namespace VehicleRepair.Application.DTOs.Customers;

public record CustomerDto(
    Guid Id,
    string INN,
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email,
    bool IsActive,
    DateTime CreatedAt
);

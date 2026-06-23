namespace VehicleRepair.Application.DTOs.Customers;

public record UpdateCustomerRequest(
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email,
    bool IsActive
);

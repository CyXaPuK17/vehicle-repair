namespace VehicleRepair.Application.DTOs.Customers;

public record CreateCustomerRequest(
    string INN,
    string Name,
    string? ContactPerson,
    string? Phone,
    string? Email
);

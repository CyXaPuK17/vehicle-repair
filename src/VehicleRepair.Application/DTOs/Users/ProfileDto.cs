using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Users;

public record ProfileDto(
    string Login,
    UserRole Role,
    string? Name,
    string? INN,
    string? ContactPerson,
    string? Address,
    string? Phone,
    string? Email,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);

using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Users;

public record UserDto(
    Guid Id,
    string Login,
    UserRole Role,
    Guid? CustomerId,
    Guid? ExecutorId,
    string? LinkedEntityName,
    bool IsActive,
    DateTime? LastLoginAt,
    DateTime CreatedAt
);

using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Users;

public record CreateUserRequest(
    string Login,
    string Password,
    UserRole Role,
    Guid? CustomerId,
    Guid? ExecutorId
);

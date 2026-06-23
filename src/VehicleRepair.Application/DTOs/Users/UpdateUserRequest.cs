using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.DTOs.Users;

public record UpdateUserRequest(
    string Login,
    UserRole Role,
    Guid? CustomerId,
    Guid? ExecutorId
);

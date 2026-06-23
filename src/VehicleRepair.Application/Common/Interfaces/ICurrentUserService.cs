using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Application.Common.Interfaces;

public interface ICurrentUserService
{
    Guid UserId { get; }
    UserRole Role { get; }
    Guid? LinkedEntityId { get; }
    bool IsAuthenticated { get; }
}

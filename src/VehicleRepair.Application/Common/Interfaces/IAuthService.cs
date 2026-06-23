using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Application.Common.Interfaces;

public interface IAuthService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    Guid? ValidateAccessToken(string token);
}

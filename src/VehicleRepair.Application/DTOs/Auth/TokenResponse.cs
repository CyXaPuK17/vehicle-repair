namespace VehicleRepair.Application.DTOs.Auth;

public record TokenResponse(string AccessToken, string Role, Guid UserId);

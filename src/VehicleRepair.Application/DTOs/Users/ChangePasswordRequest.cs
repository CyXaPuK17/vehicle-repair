namespace VehicleRepair.Application.DTOs.Users;

public record ChangePasswordRequest(string CurrentPassword, string NewPassword);

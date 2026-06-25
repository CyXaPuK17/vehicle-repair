using System.ComponentModel.DataAnnotations;

namespace VehicleRepair.Application.DTOs.Users;

public record UpdateProfileRequest(
    [MaxLength(500)] string? Name,
    [MaxLength(200)] string? ContactPerson,
    [MaxLength(500)] string? Address,
    [MaxLength(30)]  string? Phone,
    [MaxLength(200)] string? Email
);

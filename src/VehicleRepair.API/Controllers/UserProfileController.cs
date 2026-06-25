using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Application.UseCases.Users.ChangePassword;
using VehicleRepair.Application.UseCases.Users.GetCurrent;
using VehicleRepair.Application.UseCases.Users.GetProfile;
using VehicleRepair.Application.UseCases.Users.UpdateProfile;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/users/me")]
[Authorize]
public class UserProfileController : ControllerBase
{
    private readonly GetCurrentUserUseCase _getCurrent;
    private readonly ChangePasswordUseCase _changePassword;
    private readonly GetMyProfileUseCase _getProfile;
    private readonly UpdateMyProfileUseCase _updateProfile;
    private readonly ICurrentUserService _currentUser;

    public UserProfileController(
        GetCurrentUserUseCase getCurrent, ChangePasswordUseCase changePassword,
        GetMyProfileUseCase getProfile, UpdateMyProfileUseCase updateProfile,
        ICurrentUserService currentUser)
    {
        _getCurrent = getCurrent;
        _changePassword = changePassword;
        _getProfile = getProfile;
        _updateProfile = updateProfile;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<IActionResult> GetMe(CancellationToken ct) =>
        Ok(ApiResponse<UserDto>.Ok(await _getCurrent.ExecuteAsync(ct)));

    [HttpPatch("password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _changePassword.ExecuteAsync(_currentUser.UserId, request, ct);
        return Ok(ApiResponse<string>.Ok("Пароль изменён."));
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetMyProfile(CancellationToken ct) =>
        Ok(ApiResponse<ProfileDto>.Ok(await _getProfile.ExecuteAsync(ct)));

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateMyProfile([FromBody] UpdateProfileRequest request, CancellationToken ct)
    {
        await _updateProfile.ExecuteAsync(request, ct);
        return Ok(ApiResponse<string>.Ok("Профиль обновлён."));
    }
}

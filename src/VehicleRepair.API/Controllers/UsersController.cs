using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Application.UseCases.Users.ChangePassword;
using VehicleRepair.Application.UseCases.Users.Create;
using VehicleRepair.Application.UseCases.Users.GetAll;
using VehicleRepair.Application.UseCases.Users.GetCurrent;
using VehicleRepair.Application.UseCases.Users.SetActive;
using VehicleRepair.Application.UseCases.Users.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _create;
    private readonly UpdateUserUseCase _update;
    private readonly GetAllUsersUseCase _getAll;
    private readonly GetCurrentUserUseCase _getCurrent;
    private readonly ChangePasswordUseCase _changePassword;
    private readonly SetUserActiveUseCase _setActive;
    private readonly ICurrentUserService _currentUser;

    public UsersController(
        CreateUserUseCase create, UpdateUserUseCase update, GetAllUsersUseCase getAll,
        GetCurrentUserUseCase getCurrent, ChangePasswordUseCase changePassword,
        SetUserActiveUseCase setActive, ICurrentUserService currentUser)
    {
        _create = create;
        _update = update;
        _getAll = getAll;
        _getCurrent = getCurrent;
        _changePassword = changePassword;
        _setActive = setActive;
        _currentUser = currentUser;
    }

    [HttpGet]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<UserDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Пользователь обновлён."));
    }

    [HttpPost]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPatch("{id:guid}/active")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetUserActiveRequest request, CancellationToken ct)
    {
        await _setActive.ExecuteAsync(id, request.IsActive, ct);
        return Ok(ApiResponse<string>.Ok(request.IsActive ? "Пользователь активирован." : "Пользователь деактивирован."));
    }

    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken ct) =>
        Ok(ApiResponse<UserDto>.Ok(await _getCurrent.ExecuteAsync(ct)));

    [HttpPatch("me/password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request, CancellationToken ct)
    {
        await _changePassword.ExecuteAsync(_currentUser.UserId, request, ct);
        return Ok(ApiResponse<string>.Ok("Пароль изменён."));
    }
}

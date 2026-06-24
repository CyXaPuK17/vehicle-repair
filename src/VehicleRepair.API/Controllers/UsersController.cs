using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Application.UseCases.Users.Create;
using VehicleRepair.Application.UseCases.Users.GetAll;
using VehicleRepair.Application.UseCases.Users.SetActive;
using VehicleRepair.Application.UseCases.Users.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/users")]
[Authorize(Roles = "ManagementCompany")]
public class UsersController : ControllerBase
{
    private readonly CreateUserUseCase _create;
    private readonly UpdateUserUseCase _update;
    private readonly GetAllUsersUseCase _getAll;
    private readonly SetUserActiveUseCase _setActive;

    public UsersController(
        CreateUserUseCase create, UpdateUserUseCase update,
        GetAllUsersUseCase getAll, SetUserActiveUseCase setActive)
    {
        _create = create;
        _update = update;
        _getAll = getAll;
        _setActive = setActive;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<UserDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Пользователь обновлён."));
    }

    [HttpPatch("{id:guid}/active")]
    public async Task<IActionResult> SetActive(Guid id, [FromBody] SetUserActiveRequest request, CancellationToken ct)
    {
        await _setActive.ExecuteAsync(id, request.IsActive, ct);
        return Ok(ApiResponse<string>.Ok(request.IsActive ? "Пользователь активирован." : "Пользователь деактивирован."));
    }
}

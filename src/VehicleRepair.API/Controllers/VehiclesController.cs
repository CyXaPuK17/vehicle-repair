using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.DTOs.Vehicles;
using VehicleRepair.Application.UseCases.Vehicles.Create;
using VehicleRepair.Application.UseCases.Vehicles.GetAll;
using VehicleRepair.Application.UseCases.Vehicles.GetHistory;
using VehicleRepair.Application.UseCases.Vehicles.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/vehicles")]
[Authorize]
public class VehiclesController : ControllerBase
{
    private readonly CreateVehicleUseCase _create;
    private readonly UpdateVehicleUseCase _update;
    private readonly GetAllVehiclesUseCase _getAll;
    private readonly GetVehicleHistoryUseCase _history;

    public VehiclesController(
        CreateVehicleUseCase create,
        UpdateVehicleUseCase update,
        GetAllVehiclesUseCase getAll,
        GetVehicleHistoryUseCase history)
    {
        _create  = create;
        _update  = update;
        _getAll  = getAll;
        _history = history;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<VehicleDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Create([FromBody] CreateVehicleRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateVehicleRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("ТС обновлено."));
    }

    [HttpGet("{id:guid}/repairs")]
    [Authorize(Roles = "ManagementCompany,Customer")]
    public async Task<IActionResult> GetHistory(Guid id, CancellationToken ct)
    {
        try
        {
            var result = await _history.ExecuteAsync(id, ct);
            return Ok(ApiResponse<IReadOnlyList<RepairDto>>.Ok(result));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
    }
}

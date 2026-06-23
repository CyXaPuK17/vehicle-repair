using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.RepairTypes;
using VehicleRepair.Application.UseCases.RepairTypes.Create;
using VehicleRepair.Application.UseCases.RepairTypes.GetAll;
using VehicleRepair.Application.UseCases.RepairTypes.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/repair-types")]
[Authorize]
public class RepairTypesController : ControllerBase
{
    private readonly CreateRepairTypeUseCase _create;
    private readonly UpdateRepairTypeUseCase _update;
    private readonly GetAllRepairTypesUseCase _getAll;

    public RepairTypesController(CreateRepairTypeUseCase create, UpdateRepairTypeUseCase update, GetAllRepairTypesUseCase getAll)
    {
        _create = create;
        _update = update;
        _getAll = getAll;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<RepairTypeDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Create([FromBody] CreateRepairTypeRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRepairTypeRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Вид ремонта обновлён."));
    }
}

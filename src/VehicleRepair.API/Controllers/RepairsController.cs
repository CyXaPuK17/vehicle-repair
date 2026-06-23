using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.Create;
using VehicleRepair.Application.UseCases.Repairs.GetAll;
using VehicleRepair.Application.UseCases.Repairs.Issue;
using VehicleRepair.Application.UseCases.Repairs.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/repairs")]
[Authorize(Roles = "ManagementCompany,Executor,Customer")]
public class RepairsController : ControllerBase
{
    private readonly CreateRepairUseCase _create;
    private readonly UpdateRepairUseCase _update;
    private readonly IssueRepairUseCase _issue;
    private readonly GetAllRepairsUseCase _getAll;

    public RepairsController(
        CreateRepairUseCase create,
        UpdateRepairUseCase update,
        IssueRepairUseCase issue,
        GetAllRepairsUseCase getAll)
    {
        _create = create;
        _update = update;
        _issue = issue;
        _getAll = getAll;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50,
        CancellationToken ct = default) =>
        Ok(ApiResponse<PagedResult<RepairDto>>.Ok(await _getAll.ExecuteAsync(from, to, page, pageSize, ct)));

    [HttpPost]
    [Authorize(Roles = "Executor")]
    public async Task<IActionResult> Create([FromBody] CreateRepairRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRepairRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Ремонт обновлён."));
    }

    [HttpPatch("{id:guid}/issue")]
    public async Task<IActionResult> Issue(Guid id, [FromBody] IssueRepairRequest request, CancellationToken ct)
    {
        await _issue.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("ТС выдано."));
    }
}

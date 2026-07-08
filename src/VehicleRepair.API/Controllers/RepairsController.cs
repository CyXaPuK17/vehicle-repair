using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.Complete;
using VehicleRepair.Application.UseCases.Repairs.Create;
using VehicleRepair.Application.UseCases.Repairs.GetAll;
using VehicleRepair.Application.UseCases.Repairs.Issue;
using VehicleRepair.Application.UseCases.Repairs.Start;
using VehicleRepair.Application.UseCases.Repairs.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/repairs")]
[Authorize(Roles = "ManagementCompany,Executor,Customer")]
public class RepairsController : ControllerBase
{
    private readonly CreateRepairUseCase _create;
    private readonly UpdateRepairUseCase _update;
    private readonly StartRepairUseCase _start;
    private readonly CompleteRepairUseCase _complete;
    private readonly IssueRepairUseCase _issue;
    private readonly GetAllRepairsUseCase _getAll;

    public RepairsController(
        CreateRepairUseCase create,
        UpdateRepairUseCase update,
        StartRepairUseCase start,
        CompleteRepairUseCase complete,
        IssueRepairUseCase issue,
        GetAllRepairsUseCase getAll)
    {
        _create = create;
        _update = update;
        _start = start;
        _complete = complete;
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

    [HttpPatch("{id:guid}/start")]
    [Authorize(Roles = "Executor")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await _start.ExecuteAsync(id, ct);
        return Ok(ApiResponse<string>.Ok("Ремонт взят в работу."));
    }

    [HttpPatch("{id:guid}/complete")]
    [Authorize(Roles = "Executor")]
    public async Task<IActionResult> Complete(Guid id, CancellationToken ct)
    {
        await _complete.ExecuteAsync(id, ct);
        return Ok(ApiResponse<string>.Ok("Ремонт завершён."));
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Executors;
using VehicleRepair.Application.UseCases.Executors.Create;
using VehicleRepair.Application.UseCases.Executors.GetAll;
using VehicleRepair.Application.UseCases.Executors.GetStats;
using VehicleRepair.Application.UseCases.Executors.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/executors")]
public class ExecutorsController : ControllerBase
{
    private readonly CreateExecutorUseCase _create;
    private readonly UpdateExecutorUseCase _update;
    private readonly GetAllExecutorsUseCase _getAll;
    private readonly GetExecutorStatsUseCase _stats;

    public ExecutorsController(
        CreateExecutorUseCase create,
        UpdateExecutorUseCase update,
        GetAllExecutorsUseCase getAll,
        GetExecutorStatsUseCase stats)
    {
        _create = create;
        _update = update;
        _getAll = getAll;
        _stats  = stats;
    }

    [HttpGet]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<ExecutorDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpPost]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Create([FromBody] CreateExecutorRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return Ok(ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateExecutorRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Исполнитель обновлён."));
    }

    [HttpGet("me/stats")]
    [Authorize(Roles = "Executor")]
    public async Task<IActionResult> GetMyStats(CancellationToken ct) =>
        Ok(ApiResponse<ExecutorStatsDto>.Ok(await _stats.ExecuteAsync(ct)));
}

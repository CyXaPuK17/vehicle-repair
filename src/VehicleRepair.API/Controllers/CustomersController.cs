using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Application.UseCases.Customers.Create;
using VehicleRepair.Application.UseCases.Customers.GetAll;
using VehicleRepair.Application.UseCases.Customers.GetById;
using VehicleRepair.Application.UseCases.Customers.GetStats;
using VehicleRepair.Application.UseCases.Customers.Update;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/customers")]
public class CustomersController : ControllerBase
{
    private readonly CreateCustomerUseCase _create;
    private readonly UpdateCustomerUseCase _update;
    private readonly GetAllCustomersUseCase _getAll;
    private readonly GetCustomerByIdUseCase _getById;
    private readonly GetCustomerStatsUseCase _stats;

    public CustomersController(
        CreateCustomerUseCase create, UpdateCustomerUseCase update,
        GetAllCustomersUseCase getAll, GetCustomerByIdUseCase getById,
        GetCustomerStatsUseCase stats)
    {
        _create  = create;
        _update  = update;
        _getAll  = getAll;
        _getById = getById;
        _stats   = stats;
    }

    [HttpGet]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> GetAll(CancellationToken ct) =>
        Ok(ApiResponse<IReadOnlyList<CustomerDto>>.Ok(await _getAll.ExecuteAsync(ct)));

    [HttpGet("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct) =>
        Ok(ApiResponse<CustomerDto>.Ok(await _getById.ExecuteAsync(id, ct)));

    [HttpPost]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Create([FromBody] CreateCustomerRequest request, CancellationToken ct)
    {
        var id = await _create.ExecuteAsync(request, ct);
        return CreatedAtAction(nameof(GetById), new { id }, ApiResponse<Guid>.Ok(id));
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerRequest request, CancellationToken ct)
    {
        await _update.ExecuteAsync(id, request, ct);
        return Ok(ApiResponse<string>.Ok("Заказчик обновлён."));
    }

    [HttpGet("me/stats")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetMyStats(CancellationToken ct) =>
        Ok(ApiResponse<CustomerStatsDto>.Ok(await _stats.ExecuteAsync(ct)));
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Dashboard;
using VehicleRepair.Application.UseCases.Dashboard;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly GetDashboardUseCase _useCase;
    private readonly GetCustomerDashboardUseCase _customerUseCase;

    public DashboardController(GetDashboardUseCase useCase, GetCustomerDashboardUseCase customerUseCase)
    {
        _useCase = useCase;
        _customerUseCase = customerUseCase;
    }

    [HttpGet]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int topCount = 5,
        CancellationToken ct = default)
    {
        var now      = DateTime.UtcNow;
        var fromDate = from?.ToUniversalTime() ?? new DateTime(now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var toDate   = to?.ToUniversalTime()   ?? now;
        return Ok(ApiResponse<DashboardDto>.Ok(await _useCase.ExecuteAsync(fromDate, toDate, topCount, ct)));
    }

    [HttpGet("customer")]
    [Authorize(Roles = "Customer")]
    public async Task<IActionResult> GetCustomer(CancellationToken ct) =>
        Ok(ApiResponse<CustomerDashboardDto>.Ok(await _customerUseCase.ExecuteAsync(ct)));
}

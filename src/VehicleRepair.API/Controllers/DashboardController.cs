using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Dashboard;
using VehicleRepair.Application.UseCases.Dashboard;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/dashboard")]
[Authorize(Roles = "ManagementCompany")]
public class DashboardController : ControllerBase
{
    private readonly GetDashboardUseCase _useCase;

    public DashboardController(GetDashboardUseCase useCase) => _useCase = useCase;

    [HttpGet]
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
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Reports;
using VehicleRepair.Application.UseCases.Reports.ByCustomer;
using VehicleRepair.Application.UseCases.Reports.ByExecutor;
using VehicleRepair.Application.UseCases.Reports.ByRepairs;
using VehicleRepair.Application.UseCases.Reports.ByVehicle;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/reports")]
[Authorize]
public class ReportsController : ControllerBase
{
    private readonly ReportByCustomerUseCase _byCustomer;
    private readonly ReportByExecutorUseCase _byExecutor;
    private readonly ReportByRepairsUseCase _byRepairs;
    private readonly ReportByVehicleUseCase _byVehicle;
    private readonly IReportExporter _excelExporter;
    private readonly IReportExporter _pdfExporter;

    public ReportsController(
        ReportByCustomerUseCase byCustomer,
        ReportByExecutorUseCase byExecutor,
        ReportByRepairsUseCase byRepairs,
        ReportByVehicleUseCase byVehicle,
        [FromKeyedServices("xlsx")] IReportExporter excelExporter,
        [FromKeyedServices("pdf")] IReportExporter pdfExporter)
    {
        _byCustomer = byCustomer;
        _byExecutor = byExecutor;
        _byRepairs = byRepairs;
        _byVehicle = byVehicle;
        _excelExporter = excelExporter;
        _pdfExporter = pdfExporter;
    }

    [HttpGet("by-customer")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> ByCustomer(
        [FromQuery] Guid? customerId,
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? export,
        CancellationToken ct)
    {
        var data = await _byCustomer.ExecuteAsync(customerId, Utc(from), Utc(to, end: true), ct);
        if (export is not null) return ExportResult(export, "Отчёт по заказчику",
            new[] { "Заказчик", "ИНН", "Кол-во ремонтов", "Сумма" },
            data.Rows, r => new object[] { r.CustomerName, r.CustomerINN, r.RepairCount, r.TotalCost });
        return Ok(ApiResponse<ReportByCustomerDto>.Ok(data));
    }

    [HttpGet("by-executor")]
    [Authorize(Roles = "ManagementCompany")]
    public async Task<IActionResult> ByExecutor(
        [FromQuery] Guid? executorId,
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? export,
        CancellationToken ct)
    {
        var data = await _byExecutor.ExecuteAsync(executorId, Utc(from), Utc(to, end: true), ct);
        if (export is not null) return ExportResult(export, "Отчёт по исполнителю",
            new[] { "Исполнитель", "ИНН", "Кол-во ремонтов", "Сумма" },
            data.Rows, r => new object[] { r.ExecutorName, r.ExecutorINN, r.RepairCount, r.TotalCost });
        return Ok(ApiResponse<ReportByExecutorDto>.Ok(data));
    }

    [HttpGet("by-repairs")]
    [Authorize(Roles = "ManagementCompany,Executor")]
    public async Task<IActionResult> ByRepairs(
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? export,
        CancellationToken ct)
    {
        var data = await _byRepairs.ExecuteAsync(Utc(from), Utc(to, end: true), ct);
        if (export is not null) return ExportResult(export, "Отчёт по ремонтам",
            new[] { "Гос.номер", "ТС", "Заказчик", "Исполнитель", "Вид ремонта", "Дата приёмки", "Дата выдачи", "Пробег", "Стоимость" },
            data.Rows, r => new object[]
            {
                r.LicensePlate, r.VehicleMakeModel, r.CustomerName, r.ExecutorName,
                r.RepairTypeName, r.ReceivedAt.ToString("dd.MM.yyyy"),
                r.IssuedAt?.ToString("dd.MM.yyyy") ?? "-", r.Mileage, r.Cost
            });
        return Ok(ApiResponse<ReportByRepairsDto>.Ok(data));
    }

    [HttpGet("by-vehicle")]
    [Authorize(Roles = "ManagementCompany,Customer")]
    public async Task<IActionResult> ByVehicle(
        [FromQuery] Guid? vehicleId,
        [FromQuery] DateTime from, [FromQuery] DateTime to,
        [FromQuery] string? export,
        CancellationToken ct)
    {
        var data = await _byVehicle.ExecuteAsync(vehicleId, Utc(from), Utc(to, end: true), ct);
        if (export is not null)
        {
            var flatRows = data.Rows.SelectMany(v =>
                v.Repairs.Select(r => new object[]
                {
                    v.LicensePlate, $"{v.MakeModel}", v.CustomerName,
                    r.RepairTypeName, r.ExecutorName,
                    r.ReceivedAt.ToString("dd.MM.yyyy"), r.Mileage, r.Cost
                })).ToList();

            return ExportResult<object[]>(export, "Отчёт по ТС",
                new[] { "Гос.номер", "Марка/модель", "Заказчик", "Вид ремонта", "Исполнитель", "Дата", "Пробег", "Стоимость" },
                flatRows, r => r);
        }
        return Ok(ApiResponse<ReportByVehicleDto>.Ok(data));
    }

    private static DateTime Utc(DateTime dt, bool end = false) =>
        end
            ? DateTime.SpecifyKind(dt.Date.AddDays(1).AddTicks(-1), DateTimeKind.Utc)
            : DateTime.SpecifyKind(dt.Date, DateTimeKind.Utc);

    private IActionResult ExportResult<T>(string format, string title, string[] headers,
        IReadOnlyList<T> rows, Func<T, object[]> mapper)
    {
        if (format.Equals("xlsx", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = _excelExporter.Export(title, headers, rows, mapper);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"{title}_{DateTime.UtcNow:yyyyMMdd}.xlsx");
        }
        if (format.Equals("pdf", StringComparison.OrdinalIgnoreCase))
        {
            var bytes = _pdfExporter.Export(title, headers, rows, mapper);
            return File(bytes, "application/pdf", $"{title}_{DateTime.UtcNow:yyyyMMdd}.pdf");
        }
        return BadRequest(ApiResponse<object>.Fail("INVALID_FORMAT", "Допустимые форматы: xlsx, pdf."));
    }
}

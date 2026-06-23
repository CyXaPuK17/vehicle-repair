using VehicleRepair.Application.DTOs.Reports;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Reports.ByCustomer;

public class ReportByCustomerUseCase
{
    private readonly IUnitOfWork _uow;

    public ReportByCustomerUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<ReportByCustomerDto> ExecuteAsync(
        Guid? customerId, DateTime from, DateTime to, CancellationToken ct)
    {
        var customers = customerId.HasValue
            ? (await _uow.Customers.FindAsync(c => c.Id == customerId.Value, ct))
            : await _uow.Customers.GetActiveAsync(ct);

        var rows = new List<ReportByCustomerRow>();
        foreach (var customer in customers)
        {
            var repairs = await _uow.Repairs.GetByCustomerIdAsync(customer.Id, from, to, ct);
            rows.Add(new ReportByCustomerRow(
                customer.Id,
                customer.Name,
                customer.INN,
                repairs.Count,
                repairs.Sum(r => r.Cost)
            ));
        }

        return new ReportByCustomerDto(rows, rows.Sum(r => r.RepairCount), rows.Sum(r => r.TotalCost));
    }
}

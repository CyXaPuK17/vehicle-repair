using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Reports;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Reports.ByVehicle;

public class ReportByVehicleUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public ReportByVehicleUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<ReportByVehicleDto> ExecuteAsync(
        Guid? vehicleId, DateTime from, DateTime to, CancellationToken ct)
    {
        IReadOnlyList<Domain.Entities.Vehicle> vehicles;

        if (_currentUser.Role == UserRole.Customer && _currentUser.LinkedEntityId.HasValue)
        {
            vehicles = await _uow.Vehicles.GetByCustomerIdAsync(_currentUser.LinkedEntityId.Value, ct);
            if (vehicleId.HasValue)
            {
                vehicles = vehicles.Where(v => v.Id == vehicleId.Value).ToList();
                // if the filtered result is empty, this vehicleId doesn't belong to this customer
                if (vehicles.Count == 0)
                    throw new ForbiddenException("Нет доступа к данному ТС.");
            }
        }
        else if (vehicleId.HasValue)
        {
            var vehicle = await _uow.Vehicles.GetByIdAsync(vehicleId.Value, ct)
                ?? throw new NotFoundException(nameof(Vehicle), vehicleId.Value);
            vehicles = new[] { vehicle };
        }
        else
        {
            vehicles = await _uow.Vehicles.GetAllAsync(ct);
        }

        // Load all repairs in a single query, then group in memory (avoids N+1)
        var vehicleIds = vehicles.Select(v => v.Id).ToList();
        var allRepairs = await _uow.Repairs.GetByVehicleIdsAsync(vehicleIds, from, to, ct);
        var repairsByVehicle = allRepairs.GroupBy(r => r.VehicleId)
            .ToDictionary(g => g.Key, g => (IReadOnlyList<Domain.Entities.Repair>)g.ToList());

        var rows = new List<ReportByVehicleRow>();
        foreach (var vehicle in vehicles)
        {
            if (!repairsByVehicle.TryGetValue(vehicle.Id, out var repairs) || !repairs.Any())
                continue;

            var mileageDelta = repairs.Max(r => r.Mileage) - repairs.Min(r => r.Mileage);

            var repairRows = repairs.Select(r => new ReportByVehicleRepairRow(
                r.RepairType?.Name ?? string.Empty,
                r.Executor?.Name ?? string.Empty,
                r.ReceivedAt,
                r.Mileage,
                r.Cost
            )).ToList();

            rows.Add(new ReportByVehicleRow(
                vehicle.Id,
                vehicle.LicensePlate,
                $"{vehicle.Make} {vehicle.Model}",
                vehicle.Customer?.Name ?? string.Empty,
                repairs.Sum(r => r.Cost),
                mileageDelta,
                repairRows
            ));
        }

        return new ReportByVehicleDto(rows);
    }
}

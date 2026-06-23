using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Vehicles;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Vehicles.GetAll;

public class GetAllVehiclesUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetAllVehiclesUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<VehicleDto>> ExecuteAsync(CancellationToken ct)
    {
        var vehicles = _currentUser.Role == UserRole.Customer && _currentUser.LinkedEntityId.HasValue
            ? await _uow.Vehicles.GetByCustomerIdAsync(_currentUser.LinkedEntityId.Value, ct)
            : await _uow.Vehicles.GetAllAsync(ct);

        return vehicles.Select(v => new VehicleDto(
            v.Id, v.LicensePlate, v.Make, v.Model, v.Year, v.VIN, v.VehicleType,
            v.IsActive, v.CustomerId, v.Customer?.Name ?? string.Empty
        )).ToList();
    }
}

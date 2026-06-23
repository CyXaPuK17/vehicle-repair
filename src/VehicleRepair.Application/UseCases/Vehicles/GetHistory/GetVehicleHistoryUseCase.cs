using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.GetAll;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Vehicles.GetHistory;

public class GetVehicleHistoryUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public GetVehicleHistoryUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task<IReadOnlyList<RepairDto>> ExecuteAsync(Guid vehicleId, CancellationToken ct)
    {
        if (_currentUser.Role == UserRole.Customer)
        {
            var customerId = _currentUser.LinkedEntityId
                ?? throw new UnauthorizedAccessException();
            var vehicle = await _uow.Vehicles.GetByIdAsync(vehicleId, ct);
            if (vehicle == null || vehicle.CustomerId != customerId)
                throw new UnauthorizedAccessException();
        }

        var repairs = await _uow.Repairs.GetAllByVehicleIdAsync(vehicleId, ct);
        return repairs.Select(GetAllRepairsUseCase.MapToDto).ToList();
    }
}

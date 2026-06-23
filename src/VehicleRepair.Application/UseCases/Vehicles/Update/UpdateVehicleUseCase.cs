using VehicleRepair.Application.DTOs.Vehicles;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Vehicles.Update;

public class UpdateVehicleUseCase
{
    private readonly IUnitOfWork _uow;

    public UpdateVehicleUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task ExecuteAsync(Guid id, UpdateVehicleRequest request, CancellationToken ct)
    {
        var vehicle = await _uow.Vehicles.GetByIdAsync(id, ct)
            ?? throw new NotFoundException(nameof(Vehicle), id);

        if (!vehicle.LicensePlate.Equals(request.LicensePlate, StringComparison.OrdinalIgnoreCase)
            && await _uow.Vehicles.ExistsByLicensePlateAsync(request.LicensePlate, ct))
            throw new DomainException($"ТС с гос. номером '{request.LicensePlate}' уже существует.");

        vehicle.LicensePlate = request.LicensePlate.ToUpper();
        vehicle.Make = request.Make;
        vehicle.Model = request.Model;
        vehicle.Year = request.Year;
        vehicle.VIN = request.VIN?.ToUpper();
        vehicle.VehicleType = request.VehicleType;
        vehicle.IsActive = request.IsActive;

        await _uow.SaveChangesAsync(ct);
    }
}

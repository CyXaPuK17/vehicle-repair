using VehicleRepair.Application.DTOs.Vehicles;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Vehicles.Create;

public class CreateVehicleUseCase
{
    private readonly IUnitOfWork _uow;

    public CreateVehicleUseCase(IUnitOfWork uow) => _uow = uow;

    public async Task<Guid> ExecuteAsync(CreateVehicleRequest request, CancellationToken ct)
    {
        if (await _uow.Customers.GetByIdAsync(request.CustomerId, ct) is null)
            throw new NotFoundException(nameof(Customer), request.CustomerId);

        if (await _uow.Vehicles.ExistsByLicensePlateAsync(request.LicensePlate, ct))
            throw new DomainException($"ТС с гос. номером '{request.LicensePlate}' уже существует.");

        var vehicle = new Vehicle
        {
            Id = Guid.NewGuid(),
            LicensePlate = request.LicensePlate.ToUpper(),
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            VIN = request.VIN?.ToUpper(),
            VehicleType = request.VehicleType,
            CustomerId = request.CustomerId,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Vehicles.AddAsync(vehicle, ct);
        await _uow.SaveChangesAsync(ct);
        return vehicle.Id;
    }
}

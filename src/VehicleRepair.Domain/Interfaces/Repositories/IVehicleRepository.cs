using VehicleRepair.Domain.Entities;

namespace VehicleRepair.Domain.Interfaces.Repositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken ct = default);
    Task<IReadOnlyList<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default);
    Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken ct = default);
    Task<Vehicle?> GetWithCustomerAsync(Guid id, CancellationToken ct = default);
}

using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class VehicleRepository : Repository<Vehicle>, IVehicleRepository
{
    public VehicleRepository(AppDbContext db) : base(db) { }

    public async Task<Vehicle?> GetByLicensePlateAsync(string licensePlate, CancellationToken ct = default) =>
        await _set.FirstOrDefaultAsync(v => v.LicensePlate == licensePlate, ct);

    public async Task<IReadOnlyList<Vehicle>> GetByCustomerIdAsync(Guid customerId, CancellationToken ct = default) =>
        await _set.Include(v => v.Customer).Where(v => v.CustomerId == customerId).ToListAsync(ct);

    public async Task<bool> ExistsByLicensePlateAsync(string licensePlate, CancellationToken ct = default) =>
        await _set.AnyAsync(v => v.LicensePlate == licensePlate, ct);

    public async Task<Vehicle?> GetWithCustomerAsync(Guid id, CancellationToken ct = default) =>
        await _set.Include(v => v.Customer).FirstOrDefaultAsync(v => v.Id == id, ct);

    public override async Task<IReadOnlyList<Vehicle>> GetAllAsync(CancellationToken ct = default) =>
        await _set.Include(v => v.Customer).ToListAsync(ct);
}

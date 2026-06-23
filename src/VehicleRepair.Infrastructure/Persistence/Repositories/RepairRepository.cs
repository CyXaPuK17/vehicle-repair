using Microsoft.EntityFrameworkCore;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces.Repositories;

namespace VehicleRepair.Infrastructure.Persistence.Repositories;

public class RepairRepository : Repository<Repair>, IRepairRepository
{
    public RepairRepository(AppDbContext db) : base(db) { }

    private IQueryable<Repair> WithDetails() =>
        _set
            .Include(r => r.Vehicle).ThenInclude(v => v.Customer)
            .Include(r => r.Executor)
            .Include(r => r.RepairType);

    public async Task<IReadOnlyList<Repair>> GetByExecutorIdAsync(Guid executorId, DateTime from, DateTime to, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.ExecutorId == executorId && r.ReceivedAt >= from && r.ReceivedAt <= to)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetByVehicleIdAsync(Guid vehicleId, DateTime from, DateTime to, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.VehicleId == vehicleId && r.ReceivedAt >= from && r.ReceivedAt <= to)
            .OrderBy(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetByVehicleIdsAsync(IEnumerable<Guid> vehicleIds, DateTime from, DateTime to, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => vehicleIds.Contains(r.VehicleId) && r.ReceivedAt >= from && r.ReceivedAt <= to)
            .OrderBy(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetByCustomerIdAsync(Guid customerId, DateTime from, DateTime to, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.Vehicle.CustomerId == customerId && r.ReceivedAt >= from && r.ReceivedAt <= to)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.ReceivedAt >= from && r.ReceivedAt <= to)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<Repair?> GetWithDetailsAsync(Guid id, CancellationToken ct = default) =>
        await WithDetails().FirstOrDefaultAsync(r => r.Id == id, ct);

    public async Task<IReadOnlyList<Repair>> GetAllActiveAsync(CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.Status != RepairStatus.Issued)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetActiveByCustomerIdAsync(Guid customerId, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.Vehicle.CustomerId == customerId && r.Status != RepairStatus.Issued)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);

    public async Task<IReadOnlyList<Repair>> GetAllByVehicleIdAsync(Guid vehicleId, CancellationToken ct = default) =>
        await WithDetails()
            .Where(r => r.VehicleId == vehicleId)
            .OrderByDescending(r => r.ReceivedAt)
            .ToListAsync(ct);
}

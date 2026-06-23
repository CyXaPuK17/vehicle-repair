using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Domain.Entities;

public class Repair
{
    public Guid Id { get; set; }
    public Guid VehicleId { get; set; }
    public Guid ExecutorId { get; set; }
    public Guid RepairTypeId { get; set; }
    public DateTime ReceivedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public decimal Cost { get; set; }
    public int Mileage { get; set; }
    public RepairStatus Status { get; set; } = RepairStatus.Received;
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }

    public Vehicle Vehicle { get; set; } = null!;
    public Executor Executor { get; set; } = null!;
    public RepairType RepairType { get; set; } = null!;
    public User CreatedByUser { get; set; } = null!;
}

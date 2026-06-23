namespace VehicleRepair.Domain.Entities;

public class RepairType
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; } = true;

    public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}

namespace VehicleRepair.Domain.Entities;

public class Executor
{
    public Guid Id { get; set; }
    public string INN { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
    public ICollection<User> Users { get; set; } = new List<User>();
}

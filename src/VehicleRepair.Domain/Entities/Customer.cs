namespace VehicleRepair.Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string INN { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string? ContactPerson { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
    public ICollection<User> Users { get; set; } = new List<User>();
}

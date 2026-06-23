using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Domain.Entities;

public class Vehicle
{
    public Guid Id { get; set; }
    public string LicensePlate { get; set; } = null!;
    public string Make { get; set; } = null!;
    public string Model { get; set; } = null!;
    public int? Year { get; set; }
    public string? VIN { get; set; }
    public VehicleType VehicleType { get; set; } = VehicleType.Passenger;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public Guid CustomerId { get; set; }
    public Customer Customer { get; set; } = null!;

    public ICollection<Repair> Repairs { get; set; } = new List<Repair>();
}

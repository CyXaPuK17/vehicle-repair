using VehicleRepair.Domain.Enums;

namespace VehicleRepair.Domain.Entities;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    public UserRole Role { get; set; }
    public Guid? CustomerId { get; set; }
    public Guid? ExecutorId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAt { get; set; }
    public DateTime CreatedAt { get; set; }

    public Customer? Customer { get; set; }
    public Executor? Executor { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

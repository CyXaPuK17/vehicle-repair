using System.Security.Claims;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Domain.Enums;

namespace VehicleRepair.API.Middleware;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; private set; }
    public UserRole Role { get; private set; }
    public Guid? LinkedEntityId { get; private set; }
    public bool IsAuthenticated { get; private set; }

    public void SetFromClaims(ClaimsPrincipal principal)
    {
        IsAuthenticated = principal.Identity?.IsAuthenticated ?? false;
        if (!IsAuthenticated) return;

        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier) ?? principal.FindFirstValue("sub");
        if (string.IsNullOrEmpty(sub))
            throw new InvalidOperationException("Authenticated principal is missing the 'sub' claim.");
        UserId = Guid.Parse(sub);

        if (Enum.TryParse<UserRole>(principal.FindFirstValue(ClaimTypes.Role), out var role))
            Role = role;

        var linkedRaw = principal.FindFirstValue("linkedEntityId");
        LinkedEntityId = Guid.TryParse(linkedRaw, out var lid) && lid != Guid.Empty ? lid : null;
    }
}

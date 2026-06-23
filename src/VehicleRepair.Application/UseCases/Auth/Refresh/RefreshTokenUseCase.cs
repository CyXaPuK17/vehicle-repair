using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Auth;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Auth.Refresh;

public class RefreshTokenUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly IAuthService _auth;

    public RefreshTokenUseCase(IUnitOfWork uow, IAuthService auth)
    {
        _uow = uow;
        _auth = auth;
    }

    public async Task<(TokenResponse token, string newRefreshToken)?> ExecuteAsync(string oldToken, CancellationToken ct)
    {
        var existing = await _uow.RefreshTokens.GetActiveByTokenAsync(oldToken, ct);
        if (existing is null || existing.ExpiresAt < DateTime.UtcNow)
            return null;

        var user = await _uow.Users.GetByIdAsync(existing.UserId, ct);
        if (user is null || !user.IsActive)
            return null;

        existing.IsRevoked = true;

        var newRefresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _auth.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _uow.RefreshTokens.AddAsync(newRefresh, ct);
        await _uow.SaveChangesAsync(ct);

        var tokenResponse = new TokenResponse(_auth.GenerateAccessToken(user), user.Role.ToString(), user.Id);
        return (tokenResponse, newRefresh.Token);
    }
}

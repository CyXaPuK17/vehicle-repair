using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Auth;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Auth.Login;

public class LoginUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly IPasswordHasher _hasher;
    private readonly IAuthService _auth;

    public LoginUseCase(IUnitOfWork uow, IPasswordHasher hasher, IAuthService auth)
    {
        _uow = uow;
        _hasher = hasher;
        _auth = auth;
    }

    public async Task<(TokenResponse token, string refreshToken)?> ExecuteAsync(LoginRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetByLoginAsync(request.Login, ct);
        if (user is null || !user.IsActive || !_hasher.Verify(request.Password, user.PasswordHash))
            return null;

        user.LastLoginAt = DateTime.UtcNow;

        var refresh = new RefreshToken
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            Token = _auth.GenerateRefreshToken(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow
        };

        await _uow.RefreshTokens.AddAsync(refresh, ct);
        await _uow.SaveChangesAsync(ct);

        var tokenResponse = new TokenResponse(_auth.GenerateAccessToken(user), user.Role.ToString(), user.Id);
        return (tokenResponse, refresh.Token);
    }
}

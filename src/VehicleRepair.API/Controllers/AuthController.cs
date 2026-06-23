using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Application.DTOs.Auth;
using VehicleRepair.Application.UseCases.Auth.Login;
using VehicleRepair.Application.UseCases.Auth.Refresh;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.API.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly LoginUseCase _login;
    private readonly RefreshTokenUseCase _refresh;
    private readonly IUnitOfWork _uow;

    public AuthController(LoginUseCase login, RefreshTokenUseCase refresh, IUnitOfWork uow)
    {
        _login = login;
        _refresh = refresh;
        _uow = uow;
    }

    [HttpPost("login")]
    [EnableRateLimiting("auth-login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await _login.ExecuteAsync(request, ct);
        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("INVALID_CREDENTIALS", "Неверный логин или пароль."));

        SetRefreshTokenCookie(result.Value.refreshToken);
        return Ok(ApiResponse<TokenResponse>.Ok(result.Value.token));
    }

    [HttpPost("refresh")]
    [EnableRateLimiting("auth-refresh")]
    public async Task<IActionResult> Refresh(CancellationToken ct)
    {
        var oldToken = Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(oldToken))
            return Unauthorized(ApiResponse<object>.Fail("NO_REFRESH_TOKEN", "Refresh token не найден."));

        var result = await _refresh.ExecuteAsync(oldToken, ct);
        if (result is null)
            return Unauthorized(ApiResponse<object>.Fail("INVALID_REFRESH_TOKEN", "Refresh token недействителен."));

        SetRefreshTokenCookie(result.Value.newRefreshToken);
        return Ok(ApiResponse<TokenResponse>.Ok(result.Value.token));
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        var token = Request.Cookies["refreshToken"];
        if (!string.IsNullOrEmpty(token))
        {
            var existing = await _uow.RefreshTokens.GetActiveByTokenAsync(token, ct);
            if (existing is not null)
            {
                existing.IsRevoked = true;
                await _uow.SaveChangesAsync(ct);
            }
        }
        Response.Cookies.Delete("refreshToken");
        return Ok(ApiResponse<string>.Ok("Выход выполнен."));
    }

    private void SetRefreshTokenCookie(string token)
    {
        Response.Cookies.Append("refreshToken", token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
    }
}

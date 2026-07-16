using System.Net;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Infrastructure.Persistence;

namespace VehicleRepair.API.Middleware;

// Токен живёт до истечения срока действия сам по себе, поэтому деактивацию пользователя
// (в т.ч. каскадную — см. SetUserActiveUseCase) нужно перепроверять на каждый запрос,
// а не только в момент логина, иначе уже выданный JWT продолжит работать после деактивации.
public class ActiveUserMiddleware
{
    private readonly RequestDelegate _next;

    public ActiveUserMiddleware(RequestDelegate next) => _next = next;

    public async Task InvokeAsync(HttpContext context, AppDbContext db)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue("sub");
            if (!Guid.TryParse(sub, out var userId))
            {
                await Reject(context);
                return;
            }

            var isActive = await db.Users
                .Where(u => u.Id == userId)
                .Select(u => (bool?)u.IsActive)
                .FirstOrDefaultAsync();

            if (isActive != true)
            {
                await Reject(context);
                return;
            }
        }

        await _next(context);
    }

    private static async Task Reject(HttpContext context)
    {
        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
        context.Response.ContentType = "application/json";
        var response = ApiResponse<object>.Fail("ACCOUNT_DISABLED", "Учётная запись деактивирована. Обратитесь к администратору.");
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

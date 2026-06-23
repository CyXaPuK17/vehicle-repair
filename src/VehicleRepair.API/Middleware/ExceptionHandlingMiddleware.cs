using System.Net;
using System.Text.Json;
using VehicleRepair.Application.Common.Models;
using VehicleRepair.Domain.Exceptions;

namespace VehicleRepair.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var (statusCode, code) = ex switch
        {
            NotFoundException => (HttpStatusCode.NotFound, "NOT_FOUND"),
            ForbiddenException => (HttpStatusCode.Forbidden, "FORBIDDEN"),
            DomainException => (HttpStatusCode.BadRequest, "DOMAIN_ERROR"),
            _ => (HttpStatusCode.InternalServerError, "INTERNAL_ERROR")
        };

        if (statusCode == HttpStatusCode.InternalServerError)
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
        else
            _logger.LogWarning(ex, "Domain exception on {Method} {Path}", context.Request.Method, context.Request.Path);

        var message = statusCode == HttpStatusCode.InternalServerError
            ? "Внутренняя ошибка сервера."
            : ex.Message;
        var response = ApiResponse<object>.Fail(code, message);
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }));
    }
}

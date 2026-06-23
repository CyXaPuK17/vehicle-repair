using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Domain.Interfaces;
using VehicleRepair.Infrastructure.Auth;
using VehicleRepair.Infrastructure.Export;
using VehicleRepair.Infrastructure.Persistence;
using VehicleRepair.Infrastructure.Seed;

namespace VehicleRepair.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AppDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IAuthService, JwtService>();
        services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
        services.AddScoped<DatabaseSeeder>();
        services.AddKeyedScoped<IReportExporter, ExcelReportExporter>("xlsx");
        services.AddKeyedScoped<IReportExporter, PdfReportExporter>("pdf");

        return services;
    }
}

using Microsoft.Extensions.DependencyInjection;
using VehicleRepair.Application.UseCases.Auth.Login;
using VehicleRepair.Application.UseCases.Dashboard;
using VehicleRepair.Application.UseCases.Auth.Refresh;
using VehicleRepair.Application.UseCases.Customers.Create;
using VehicleRepair.Application.UseCases.Customers.GetAll;
using VehicleRepair.Application.UseCases.Customers.GetById;
using VehicleRepair.Application.UseCases.Customers.GetStats;
using VehicleRepair.Application.UseCases.Customers.Update;
using VehicleRepair.Application.UseCases.Executors.Create;
using VehicleRepair.Application.UseCases.Executors.GetAll;
using VehicleRepair.Application.UseCases.Executors.GetStats;
using VehicleRepair.Application.UseCases.Executors.Update;
using VehicleRepair.Application.UseCases.Repairs.Create;
using VehicleRepair.Application.UseCases.Repairs.GetAll;
using VehicleRepair.Application.UseCases.Repairs.Issue;
using VehicleRepair.Application.UseCases.Repairs.Update;
using VehicleRepair.Application.UseCases.RepairTypes.Create;
using VehicleRepair.Application.UseCases.RepairTypes.GetAll;
using VehicleRepair.Application.UseCases.RepairTypes.Update;
using VehicleRepair.Application.UseCases.Reports.ByCustomer;
using VehicleRepair.Application.UseCases.Reports.ByExecutor;
using VehicleRepair.Application.UseCases.Reports.ByRepairs;
using VehicleRepair.Application.UseCases.Reports.ByVehicle;
using VehicleRepair.Application.UseCases.Users.ChangePassword;
using VehicleRepair.Application.UseCases.Users.Create;
using VehicleRepair.Application.UseCases.Users.GetAll;
using VehicleRepair.Application.UseCases.Users.GetCurrent;
using VehicleRepair.Application.UseCases.Users.SetActive;
using VehicleRepair.Application.UseCases.Users.Update;
using VehicleRepair.Application.UseCases.Vehicles.Create;
using VehicleRepair.Application.UseCases.Vehicles.GetAll;
using VehicleRepair.Application.UseCases.Vehicles.GetHistory;
using VehicleRepair.Application.UseCases.Vehicles.Update;

namespace VehicleRepair.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<LoginUseCase>();
        services.AddScoped<RefreshTokenUseCase>();

        services.AddScoped<CreateCustomerUseCase>();
        services.AddScoped<GetCustomerStatsUseCase>();
        services.AddScoped<UpdateCustomerUseCase>();
        services.AddScoped<GetAllCustomersUseCase>();
        services.AddScoped<GetCustomerByIdUseCase>();

        services.AddScoped<CreateExecutorUseCase>();
        services.AddScoped<GetExecutorStatsUseCase>();
        services.AddScoped<UpdateExecutorUseCase>();
        services.AddScoped<GetAllExecutorsUseCase>();

        services.AddScoped<CreateVehicleUseCase>();
        services.AddScoped<UpdateVehicleUseCase>();
        services.AddScoped<GetAllVehiclesUseCase>();
        services.AddScoped<GetVehicleHistoryUseCase>();

        services.AddScoped<CreateRepairTypeUseCase>();
        services.AddScoped<UpdateRepairTypeUseCase>();
        services.AddScoped<GetAllRepairTypesUseCase>();

        services.AddScoped<CreateRepairUseCase>();
        services.AddScoped<UpdateRepairUseCase>();
        services.AddScoped<IssueRepairUseCase>();
        services.AddScoped<GetAllRepairsUseCase>();

        services.AddScoped<GetDashboardUseCase>();

        services.AddScoped<ReportByCustomerUseCase>();
        services.AddScoped<ReportByExecutorUseCase>();
        services.AddScoped<ReportByRepairsUseCase>();
        services.AddScoped<ReportByVehicleUseCase>();

        services.AddScoped<CreateUserUseCase>();
        services.AddScoped<GetCurrentUserUseCase>();
        services.AddScoped<UpdateUserUseCase>();
        services.AddScoped<GetAllUsersUseCase>();
        services.AddScoped<ChangePasswordUseCase>();
        services.AddScoped<SetUserActiveUseCase>();

        return services;
    }
}

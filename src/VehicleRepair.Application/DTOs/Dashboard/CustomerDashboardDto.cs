namespace VehicleRepair.Application.DTOs.Dashboard;

public record CustomerDashboardDto(
    int VehicleCount,
    int ActiveRepairs,
    decimal SpentForYear
);

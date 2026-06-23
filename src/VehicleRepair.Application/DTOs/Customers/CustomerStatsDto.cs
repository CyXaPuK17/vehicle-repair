namespace VehicleRepair.Application.DTOs.Customers;

public record CustomerStatsDto(
    int VehicleCount,
    int ActiveRepairs,
    int RepairsThisYear,
    decimal RevenueThisYear
);

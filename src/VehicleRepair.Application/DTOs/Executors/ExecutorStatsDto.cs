namespace VehicleRepair.Application.DTOs.Executors;

public record ExecutorStatsDto(
    int ActiveNow,
    int DoneThisMonth,
    int DoneThisYear,
    decimal RevenueThisMonth,
    decimal RevenueThisYear
);

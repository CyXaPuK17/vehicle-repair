namespace VehicleRepair.Application.DTOs.Dashboard;

public record DashboardDto(
    int Received,
    int InProgress,
    int Completed,
    int RepairsForPeriod,
    decimal RevenueForPeriod,
    IReadOnlyList<ExecutorStatDto> TopExecutors
);

public record ExecutorStatDto(string Name, int Count, decimal TotalCost);

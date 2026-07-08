namespace VehicleRepair.Desktop.Services;

public class VehicleDto
{
    public string Id { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int? Year { get; set; }
    public string VehicleType { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string CustomerId { get; set; } = "";
}

public class ExecutorDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string INN { get; set; } = "";
}

public class RepairTypeDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public bool IsActive { get; set; }
}

public class RepairDto
{
    public string Id { get; set; } = "";
    public string VehicleId { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string VehicleMakeModel { get; set; } = "";
    public string CustomerName { get; set; } = "";
    public string ExecutorId { get; set; } = "";
    public string ExecutorName { get; set; } = "";
    public string RepairTypeId { get; set; } = "";
    public string RepairTypeName { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
    public DateTime? IssuedAt { get; set; }
    public decimal Cost { get; set; }
    public int Mileage { get; set; }
    public string Status { get; set; } = "";
    public string? Comment { get; set; }

    public string StatusLabel => Status switch
    {
        "Received" => "Принят",
        "InProgress" => "В работе",
        "Completed" => "Завершён",
        "Issued" => "Выдан",
        _ => "—"
    };
}

public class PagedResultDto<T>
{
    public List<T> Items { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int Total { get; set; }
}

public class CreateRepairRequest
{
    public string VehicleId { get; set; } = "";
    public string RepairTypeId { get; set; } = "";
    public DateTime ReceivedAt { get; set; }
    public decimal Cost { get; set; }
    public int Mileage { get; set; }
    public string? Comment { get; set; }
}

public class IssueRepairRequest
{
    public DateTime IssuedAt { get; set; }
}

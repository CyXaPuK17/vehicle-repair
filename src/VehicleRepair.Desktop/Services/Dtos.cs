namespace VehicleRepair.Desktop.Services;

public class VehicleDto
{
    public string Id { get; set; } = "";
    public string LicensePlate { get; set; } = "";
    public string Make { get; set; } = "";
    public string Model { get; set; } = "";
    public int? Year { get; set; }
    public int VehicleType { get; set; }
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
    public int Status { get; set; }
    public string? Comment { get; set; }

    public string StatusLabel => Status switch
    {
        1 => "Принят",
        2 => "В работе",
        3 => "Завершён",
        4 => "Выдан",
        _ => "—"
    };
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

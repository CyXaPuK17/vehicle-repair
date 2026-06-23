namespace VehicleRepair.Domain.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key)
        : base($"{entityName} с идентификатором '{key}' не найден.") { }
}

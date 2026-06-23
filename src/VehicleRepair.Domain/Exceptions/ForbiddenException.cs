namespace VehicleRepair.Domain.Exceptions;

public class ForbiddenException : DomainException
{
    public ForbiddenException() : base("Доступ запрещён.") { }
    public ForbiddenException(string message) : base(message) { }
}

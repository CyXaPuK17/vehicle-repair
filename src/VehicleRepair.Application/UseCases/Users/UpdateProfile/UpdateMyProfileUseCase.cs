using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Users;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.UseCases.Users.UpdateProfile;

public class UpdateMyProfileUseCase
{
    private readonly IUnitOfWork _uow;
    private readonly ICurrentUserService _currentUser;

    public UpdateMyProfileUseCase(IUnitOfWork uow, ICurrentUserService currentUser)
    {
        _uow = uow;
        _currentUser = currentUser;
    }

    public async Task ExecuteAsync(UpdateProfileRequest request, CancellationToken ct)
    {
        var user = await _uow.Users.GetWithDetailsAsync(_currentUser.UserId, ct)
            ?? throw new NotFoundException("User", _currentUser.UserId);

        if (user.Customer is { } customer)
        {
            customer.Name          = request.Name          ?? customer.Name;
            customer.ContactPerson = request.ContactPerson ?? customer.ContactPerson;
            customer.Phone         = request.Phone         ?? customer.Phone;
            customer.Email         = request.Email         ?? customer.Email;
            _uow.Customers.Update(customer);
        }
        else if (user.Executor is { } executor)
        {
            executor.Name    = request.Name    ?? executor.Name;
            executor.Address = request.Address ?? executor.Address;
            executor.Phone   = request.Phone   ?? executor.Phone;
            executor.Email   = request.Email   ?? executor.Email;
            _uow.Executors.Update(executor);
        }
        else
        {
            return;
        }

        await _uow.SaveChangesAsync(ct);
    }
}

using NSubstitute;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.Create;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class CreateRepairUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly CreateRepairUseCase _sut;

    private static readonly Guid VehicleId = Guid.NewGuid();
    private static readonly Guid RepairTypeId = Guid.NewGuid();
    private static readonly Guid ExecutorId = Guid.NewGuid();

    public CreateRepairUseCaseTests()
    {
        _sut = new CreateRepairUseCase(_uow, _currentUser);
        _uow.Vehicles.GetByIdAsync(VehicleId, Arg.Any<CancellationToken>())
            .Returns(new Vehicle { Id = VehicleId, LicensePlate = "А001АА77", Make = "ГАЗ", Model = "Газель" });
        _uow.RepairTypes.GetByIdAsync(RepairTypeId, Arg.Any<CancellationToken>())
            .Returns(new RepairType { Id = RepairTypeId, Name = "ТО" });
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_role_is_ManagementCompany()
    {
        _currentUser.Role.Returns(UserRole.ManagementCompany);

        var request = ValidRequest();

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.ExecuteAsync(request, default));
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_role_is_Customer()
    {
        _currentUser.Role.Returns(UserRole.Customer);

        var request = ValidRequest();

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.ExecuteAsync(request, default));
    }

    [Fact]
    public async Task Creates_repair_with_status_Received_for_Executor_role()
    {
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(ExecutorId);
        _currentUser.UserId.Returns(Guid.NewGuid());

        var request = ValidRequest();

        var id = await _sut.ExecuteAsync(request, default);

        Assert.NotEqual(Guid.Empty, id);
        await _uow.Repairs.Received(1).AddAsync(
            Arg.Is<Repair>(r => r.Status == RepairStatus.Received && r.ExecutorId == ExecutorId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Throws_DomainException_for_negative_mileage()
    {
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(ExecutorId);

        var request = new CreateRepairRequest(VehicleId, RepairTypeId, DateTime.Today, 1000m, -1, null);

        await Assert.ThrowsAsync<DomainException>(() => _sut.ExecuteAsync(request, default));
    }

    [Fact]
    public async Task Throws_NotFoundException_when_vehicle_not_found()
    {
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(ExecutorId);
        var missingId = Guid.NewGuid();
        _uow.Vehicles.GetByIdAsync(missingId, Arg.Any<CancellationToken>()).Returns((Vehicle?)null);

        var request = new CreateRepairRequest(missingId, RepairTypeId, DateTime.Today, 1000m, 50000, null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ExecuteAsync(request, default));
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_executor_has_no_linked_entity()
    {
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns((Guid?)null);

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.ExecuteAsync(ValidRequest(), default));
    }

    private static CreateRepairRequest ValidRequest() =>
        new(VehicleId, RepairTypeId, DateTime.Today, 5000m, 100000, null);
}

using NSubstitute;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.Update;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class UpdateRepairUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly UpdateRepairUseCase _sut;

    private static readonly Guid RepairId = Guid.NewGuid();
    private static readonly Guid ExecutorId = Guid.NewGuid();
    private static readonly Guid RepairTypeId = Guid.NewGuid();

    public UpdateRepairUseCaseTests()
    {
        _sut = new UpdateRepairUseCase(_uow, _currentUser);
        _uow.RepairTypes.GetByIdAsync(RepairTypeId, Arg.Any<CancellationToken>())
            .Returns(new RepairType { Id = RepairTypeId, Name = "ТО" });
    }

    [Fact]
    public async Task Throws_DomainException_when_repair_is_already_issued()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.Issued));
        _currentUser.Role.Returns(UserRole.ManagementCompany);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => _sut.ExecuteAsync(RepairId, ValidRequest(), default));

        Assert.Contains("выданный", ex.Message);
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_executor_edits_foreign_repair()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.InProgress));
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(Guid.NewGuid()); // different executor

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.ExecuteAsync(RepairId, ValidRequest(), default));
    }

    [Fact]
    public async Task Throws_NotFoundException_when_repair_not_found()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns((Repair?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ExecuteAsync(RepairId, ValidRequest(), default));
    }

    [Fact]
    public async Task Updates_repair_fields_on_success()
    {
        var repair = BuildRepair(RepairStatus.InProgress);
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns(repair);
        _currentUser.Role.Returns(UserRole.ManagementCompany);

        var newDate = new DateTime(2025, 3, 1);
        var request = new UpdateRepairRequest(RepairTypeId, newDate, 9999m, 75000, "тест");

        await _sut.ExecuteAsync(RepairId, request, default);

        Assert.Equal(9999m, repair.Cost);
        Assert.Equal(75000, repair.Mileage);
        Assert.Equal(newDate, repair.ReceivedAt);
        Assert.Equal("тест", repair.Comment);
        _uow.Repairs.Received(1).Update(repair);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Executor_can_update_own_repair()
    {
        var repair = BuildRepair(RepairStatus.Received);
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns(repair);
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(ExecutorId); // same executor

        await _sut.ExecuteAsync(RepairId, ValidRequest(), default);

        _uow.Repairs.Received(1).Update(repair);
    }

    private static UpdateRepairRequest ValidRequest() =>
        new(RepairTypeId, DateTime.Today, 5000m, 60000, null);

    private static Repair BuildRepair(RepairStatus status) => new()
    {
        Id = RepairId,
        ExecutorId = ExecutorId,
        VehicleId = Guid.NewGuid(),
        RepairTypeId = RepairTypeId,
        ReceivedAt = new DateTime(2025, 1, 10),
        Cost = 1000m,
        Mileage = 50000,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        CreatedByUserId = Guid.NewGuid()
    };
}

using NSubstitute;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.UseCases.Repairs.Start;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class StartRepairUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly StartRepairUseCase _sut;

    private static readonly Guid RepairId = Guid.NewGuid();
    private static readonly Guid ExecutorId = Guid.NewGuid();

    public StartRepairUseCaseTests()
    {
        _sut = new StartRepairUseCase(_uow, _currentUser);
    }

    [Fact]
    public async Task Throws_NotFoundException_when_repair_not_found()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns((Repair?)null);

        await Assert.ThrowsAsync<NotFoundException>(() => _sut.ExecuteAsync(RepairId, default));
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_executor_does_not_own_repair()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.Received));
        _currentUser.LinkedEntityId.Returns(Guid.NewGuid()); // different executor

        await Assert.ThrowsAsync<ForbiddenException>(() => _sut.ExecuteAsync(RepairId, default));
    }

    [Fact]
    public async Task Throws_DomainException_when_status_is_not_Received()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.InProgress));
        _currentUser.LinkedEntityId.Returns(ExecutorId);

        var ex = await Assert.ThrowsAsync<DomainException>(() => _sut.ExecuteAsync(RepairId, default));
        Assert.Contains("принятый", ex.Message);
    }

    [Fact]
    public async Task Starts_repair_and_sets_status_to_InProgress()
    {
        var repair = BuildRepair(RepairStatus.Received);
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns(repair);
        _currentUser.LinkedEntityId.Returns(ExecutorId);

        await _sut.ExecuteAsync(RepairId, default);

        Assert.Equal(RepairStatus.InProgress, repair.Status);
        _uow.Repairs.Received(1).Update(repair);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    private static Repair BuildRepair(RepairStatus status) => new()
    {
        Id = RepairId,
        ExecutorId = ExecutorId,
        VehicleId = Guid.NewGuid(),
        RepairTypeId = Guid.NewGuid(),
        ReceivedAt = new DateTime(2025, 1, 10),
        Cost = 1000m,
        Mileage = 50000,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        CreatedByUserId = Guid.NewGuid()
    };
}

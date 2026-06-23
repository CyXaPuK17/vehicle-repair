using NSubstitute;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Repairs;
using VehicleRepair.Application.UseCases.Repairs.Issue;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class IssueRepairUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly ICurrentUserService _currentUser = Substitute.For<ICurrentUserService>();
    private readonly IssueRepairUseCase _sut;

    private static readonly Guid RepairId = Guid.NewGuid();
    private static readonly Guid ExecutorId = Guid.NewGuid();
    private static readonly DateTime ReceivedAt = new(2025, 1, 10);

    public IssueRepairUseCaseTests()
    {
        _sut = new IssueRepairUseCase(_uow, _currentUser);
    }

    [Fact]
    public async Task Throws_DomainException_when_repair_already_issued()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.Issued));
        _currentUser.Role.Returns(UserRole.ManagementCompany);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => _sut.ExecuteAsync(RepairId, new IssueRepairRequest(ReceivedAt.AddDays(5)), default));

        Assert.Contains("уже выдан", ex.Message);
    }

    [Fact]
    public async Task Throws_ForbiddenException_when_executor_does_not_own_repair()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.InProgress));
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(Guid.NewGuid()); // different executor

        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.ExecuteAsync(RepairId, new IssueRepairRequest(ReceivedAt.AddDays(5)), default));
    }

    [Fact]
    public async Task Throws_DomainException_when_issued_date_is_before_received_date()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>())
            .Returns(BuildRepair(RepairStatus.InProgress));
        _currentUser.Role.Returns(UserRole.ManagementCompany);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => _sut.ExecuteAsync(RepairId, new IssueRepairRequest(ReceivedAt.AddDays(-1)), default));

        Assert.Contains("приёмки", ex.Message);
    }

    [Fact]
    public async Task Issues_repair_and_sets_status_to_Issued()
    {
        var repair = BuildRepair(RepairStatus.InProgress);
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns(repair);
        _currentUser.Role.Returns(UserRole.ManagementCompany);
        var issuedAt = ReceivedAt.AddDays(10);

        await _sut.ExecuteAsync(RepairId, new IssueRepairRequest(issuedAt), default);

        Assert.Equal(RepairStatus.Issued, repair.Status);
        Assert.Equal(issuedAt, repair.IssuedAt);
        _uow.Repairs.Received(1).Update(repair);
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Executor_can_issue_own_repair()
    {
        var repair = BuildRepair(RepairStatus.InProgress);
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns(repair);
        _currentUser.Role.Returns(UserRole.Executor);
        _currentUser.LinkedEntityId.Returns(ExecutorId); // same executor

        await _sut.ExecuteAsync(RepairId, new IssueRepairRequest(ReceivedAt.AddDays(5)), default);

        Assert.Equal(RepairStatus.Issued, repair.Status);
    }

    [Fact]
    public async Task Throws_NotFoundException_when_repair_not_found()
    {
        _uow.Repairs.GetByIdAsync(RepairId, Arg.Any<CancellationToken>()).Returns((Repair?)null);

        await Assert.ThrowsAsync<NotFoundException>(
            () => _sut.ExecuteAsync(RepairId, new IssueRepairRequest(DateTime.Today), default));
    }

    private static Repair BuildRepair(RepairStatus status) => new()
    {
        Id = RepairId,
        ExecutorId = ExecutorId,
        VehicleId = Guid.NewGuid(),
        RepairTypeId = Guid.NewGuid(),
        ReceivedAt = ReceivedAt,
        Cost = 1000m,
        Mileage = 50000,
        Status = status,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        CreatedByUserId = Guid.NewGuid()
    };
}

using NSubstitute;
using VehicleRepair.Application.DTOs.Customers;
using VehicleRepair.Application.UseCases.Customers.Create;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Exceptions;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class CreateCustomerUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly CreateCustomerUseCase _sut;

    public CreateCustomerUseCaseTests()
    {
        _sut = new CreateCustomerUseCase(_uow);
    }

    [Fact]
    public async Task Throws_DomainException_when_INN_already_exists()
    {
        _uow.Customers.ExistsByINNAsync("123456789012", Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new CreateCustomerRequest("123456789012", "ООО Ромашка", null, null, null);

        var ex = await Assert.ThrowsAsync<DomainException>(
            () => _sut.ExecuteAsync(request, default));

        Assert.Contains("123456789012", ex.Message);
    }

    [Fact]
    public async Task Creates_customer_and_returns_new_id_on_success()
    {
        _uow.Customers.ExistsByINNAsync("123456789012", Arg.Any<CancellationToken>())
            .Returns(false);

        var request = new CreateCustomerRequest("123456789012", "ООО Ромашка", "Иванов", "+7900", "test@example.com");

        var id = await _sut.ExecuteAsync(request, default);

        Assert.NotEqual(Guid.Empty, id);
        await _uow.Customers.Received(1).AddAsync(
            Arg.Is<Customer>(c => c.INN == "123456789012" && c.Name == "ООО Ромашка" && c.Id == id),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Does_not_save_when_INN_is_duplicate()
    {
        _uow.Customers.ExistsByINNAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(true);

        var request = new CreateCustomerRequest("123456789012", "ООО Ромашка", null, null, null);

        await Assert.ThrowsAsync<DomainException>(() => _sut.ExecuteAsync(request, default));

        await _uow.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}

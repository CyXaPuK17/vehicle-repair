using NSubstitute;
using VehicleRepair.Application.Common.Interfaces;
using VehicleRepair.Application.DTOs.Auth;
using VehicleRepair.Application.UseCases.Auth.Login;
using VehicleRepair.Domain.Entities;
using VehicleRepair.Domain.Enums;
using VehicleRepair.Domain.Interfaces;

namespace VehicleRepair.Application.Tests;

public class LoginUseCaseTests
{
    private readonly IUnitOfWork _uow = Substitute.For<IUnitOfWork>();
    private readonly IPasswordHasher _hasher = Substitute.For<IPasswordHasher>();
    private readonly IAuthService _auth = Substitute.For<IAuthService>();
    private readonly LoginUseCase _sut;

    public LoginUseCaseTests()
    {
        _sut = new LoginUseCase(_uow, _hasher, _auth);
    }

    [Fact]
    public async Task Returns_null_when_user_not_found()
    {
        _uow.Users.GetByLoginAsync("unknown", Arg.Any<CancellationToken>())
            .Returns((User?)null);

        var result = await _sut.ExecuteAsync(new LoginRequest("unknown", "pass"), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task Returns_null_when_password_is_wrong()
    {
        var user = ActiveUser("admin", "hash");
        _uow.Users.GetByLoginAsync("admin", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("wrong", "hash").Returns(false);

        var result = await _sut.ExecuteAsync(new LoginRequest("admin", "wrong"), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task Returns_null_when_user_is_inactive()
    {
        var user = ActiveUser("admin", "hash");
        user.IsActive = false;
        _uow.Users.GetByLoginAsync("admin", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("pass", "hash").Returns(true);

        var result = await _sut.ExecuteAsync(new LoginRequest("admin", "pass"), default);

        Assert.Null(result);
    }

    [Fact]
    public async Task Returns_token_and_saves_refresh_on_success()
    {
        var user = ActiveUser("admin", "hash");
        _uow.Users.GetByLoginAsync("admin", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("pass", "hash").Returns(true);
        _auth.GenerateAccessToken(user).Returns("access-token");
        _auth.GenerateRefreshToken().Returns("refresh-token");

        var result = await _sut.ExecuteAsync(new LoginRequest("admin", "pass"), default);

        Assert.NotNull(result);
        Assert.Equal("access-token", result.Value.token.AccessToken);
        Assert.Equal("refresh-token", result.Value.refreshToken);
        await _uow.RefreshTokens.Received(1).AddAsync(
            Arg.Is<RefreshToken>(r => r.Token == "refresh-token" && r.UserId == user.Id),
            Arg.Any<CancellationToken>());
        await _uow.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Updates_last_login_on_success()
    {
        var user = ActiveUser("admin", "hash");
        user.LastLoginAt = null;
        _uow.Users.GetByLoginAsync("admin", Arg.Any<CancellationToken>()).Returns(user);
        _hasher.Verify("pass", "hash").Returns(true);
        _auth.GenerateAccessToken(user).Returns("token");
        _auth.GenerateRefreshToken().Returns("rt");

        await _sut.ExecuteAsync(new LoginRequest("admin", "pass"), default);

        Assert.NotNull(user.LastLoginAt);
    }

    private static User ActiveUser(string login, string hash) => new()
    {
        Id = Guid.NewGuid(),
        Login = login,
        PasswordHash = hash,
        Role = UserRole.ManagementCompany,
        IsActive = true,
        CreatedAt = DateTime.UtcNow
    };
}

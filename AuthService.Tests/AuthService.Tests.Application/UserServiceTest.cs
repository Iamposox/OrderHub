using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace AuthService.Tests.Application;
public class UserServiceTest : IAsyncLifetime
{
    private IUserService _userService;
    private Mock<ITokenService> _tokenService;
    private Mock<IApplicationPasswordHasher> _passwordHasher;
    private Mock<IUnitOfWork> _uow;
    public async Task InitializeAsync()
    {
        var logger = NullLogger<UserService>.Instance;
        _passwordHasher = new Mock<IApplicationPasswordHasher>();
        _tokenService = new Mock<ITokenService>();

        _uow = new Mock<IUnitOfWork>();

        _userService = new UserService(logger, _passwordHasher.Object, _uow.Object, _tokenService.Object);

        var userRepoMock = new Mock<IUserRepository>();
        var roleRepoMock = new Mock<IRoleRepository>();

        _uow.Setup(m => m.Users).Returns(userRepoMock.Object);
        _uow.Setup(m => m.Roles).Returns(roleRepoMock.Object);
    }
    public async Task DisposeAsync()
    {
    }
    [Fact]
    public async Task RegisterUserAsyncDone()
    {
        _passwordHasher.Setup(m => m.HashPassword("Test")).Returns("hashed-pass");

        var dto = new UserCreateDto("Danila", "Test", "Admin");

        var result = await _userService.RegisterUserAsync(dto);

        result.Should().NotBeNull();
        result.Username.Should().Be("Danila");
        _uow.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }
}

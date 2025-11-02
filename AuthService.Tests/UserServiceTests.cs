using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace AuthService.Tests;
public class UserServiceTests
{

    private readonly Mock<ILogger<UserService>> _loggerMock;
    private readonly Mock<IApplicationPasswordHasher> _passwordHasherMock;
    private readonly AuthDbContext _context;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly UserService _userService;
    public UserServiceTests()
    {
        _loggerMock = new Mock<ILogger<UserService>>();
        _userRepoMock = new Mock<IUserRepository>();
        _roleRepoMock = new Mock<IRoleRepository>();
        _passwordHasherMock = new Mock<IApplicationPasswordHasher>();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
        .Options;

        _context = new AuthDbContext(options);

        _userService = new UserService(
            _loggerMock.Object,
            _passwordHasherMock.Object,
            _context,
            _userRepoMock.Object,
            _roleRepoMock.Object
        );
    }
    [Fact]
    public async Task RegisterUserAsync_WhenUserDoesntExists_CreatesUserAsync()
    {
        var userDto = new UserCreateDto("TestTEST", "password123", "Users2");
        var role = new Role { Id = Guid.NewGuid(), RoleName = userDto.Rolename };

        var hashedPassword = "hashed_password";

        _userRepoMock.Setup(x => x.GetByNameAsync(userDto.Username)).ReturnsAsync((User)null);

        _roleRepoMock.Setup(x => x.GetByNameAsync(userDto.Rolename))
            .ReturnsAsync(role);

        _passwordHasherMock.Setup(x => x.HashPassword(userDto.Password))
            .Returns(hashedPassword);

        //_userRepoMock.Setup(x => x.AddAsync(It.IsAny<User>())).Returns((User user) => user);

        //_contextMock.Setup(x => x.SaveChangesAsync(default)).ReturnsAsync(1);


        var result = await _userService.RegisterUserAsync(userDto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Username);

        // Проверяем вызовы зависимостей
        _userRepoMock.Verify(x => x.GetByNameAsync("testuser"), Times.Once);
        _roleRepoMock.Verify(x => x.GetByNameAsync("User"), Times.Once);
        _passwordHasherMock.Verify(x => x.HashPassword("password123"), Times.Once);
        _userRepoMock.Verify(x => x.AddAsync(It.IsAny<User>()), Times.Once);
        //_contextMock.Verify(x => x.SaveChangesAsync(default), Times.Once);
    }
}

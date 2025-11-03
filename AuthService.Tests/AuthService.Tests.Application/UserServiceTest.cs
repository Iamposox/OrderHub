using AuthService.Application.DTO;
using AuthService.Application.Services;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace AuthService.Tests.Application;
public class UserServiceTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("authtestdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();
    private AuthDbContext _context;
    private UserService _userService;
    private PasswordHasherAdapter _passwordHasher;
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseNpgsql(_postgres.GetConnectionString()) // <-- строка подключения из контейнера
        .Options;
        _context = new AuthDbContext(options);
        await _context.Database.MigrateAsync();
        var logger = NullLogger<UserService>.Instance;
        IPasswordHasher<object> hasher = new PasswordHasher<object>();
        _passwordHasher = new PasswordHasherAdapter(hasher);

        var userRepo = new UserRepository(_context);
        var roleRepo = new RoleRepository(_context);

        _userService = new UserService(logger, _passwordHasher, _context, userRepo, roleRepo);
    }
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgres.DisposeAsync().AsTask();
    }
    [Fact]
    public async Task RegisterUserAsyncDone()
    {
        var dto = new UserCreateDto("Danila", "Test", "Admin");

        var result = await _userService.RegisterUserAsync(dto);

        result.Should().NotBeNull();
        result.Username.Should().Be("Danila");

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == "Danila");
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBeNullOrWhiteSpace();

        var verify = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);
        verify.Should().NotBe(false);
    }
}

using AuthService.API.Controllers;
using AuthService.Application.DTO;
using AuthService.Application.Services;
using AuthService.Application.Settings;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace AuthService.Tests.API;
public class ApiContollerTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("authtestdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();
    private AuthDbContext _context;
    private ApiController _apiController;
    private PasswordHasherAdapter _passwordHasher;
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseNpgsql(_postgres.GetConnectionString())
        .Options;
        _context = new AuthDbContext(options);
        await _context.Database.MigrateAsync();
        var loggerUser = NullLogger<UserService>.Instance;
        IPasswordHasher<object> hasher = new PasswordHasher<object>();
        _passwordHasher = new PasswordHasherAdapter(hasher);

        var userRepo = new UserRepository(_context);
        var roleRepo = new RoleRepository(_context);

        var userService = new UserService(loggerUser, _passwordHasher, _context, userRepo, roleRepo);
        var tokenOptions = Options.Create(new TokenSettings
        {
            Key = "Lalala-lalala-lalalala-lala-lalala-la",
            Audience = "LA",
            Issuer = "LAis"
        });

        var loggerToken = NullLogger<TokenService>.Instance;

        var tokenService = new TokenService(tokenOptions, loggerToken);

        var loggerApi = NullLogger<ApiController>.Instance;
        _apiController = new ApiController(loggerApi, userService, tokenService);
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

        var result = await _apiController.RegisterAsync(dto);

        result.Should().NotBeNull();

        var user = await _context.Users
            .FirstOrDefaultAsync(x => x.Username == "Danila");
        user.Should().NotBeNull();
        user!.PasswordHash.Should().NotBeNullOrWhiteSpace();

        var verify = _passwordHasher.VerifyPassword(dto.Password, user.PasswordHash);
        verify.Should().NotBe(false);
    }
}

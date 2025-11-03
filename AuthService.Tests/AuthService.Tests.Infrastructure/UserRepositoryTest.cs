using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace AuthService.Tests.Infrastructure;
public class UserRepositoryTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("authtestdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();
    private AuthDbContext _context;
    private IUserRepository _userRepository;
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseNpgsql(_postgres.GetConnectionString()) // <-- строка подключения из контейнера
        .Options;
        _context = new AuthDbContext(options);
        await _context.Database.MigrateAsync();
        _userRepository = new UserRepository(_context);
    }
    [Fact]
    public async Task AddUser()
    {
        var user = new User { Username = "Danila", PasswordHash = "testhash", Role = new Role { RoleName = "TEst" } };
        await _userRepository.AddAsync(user);
        await _context.SaveChangesAsync();
        var userFromDb = await _context.Users.FirstOrDefaultAsync(x => x.Username == "Danila");
        userFromDb.Should().NotBeNull();
        userFromDb.Id.Should().NotBeEmpty();
        userFromDb.PasswordHash.Should().Be("testhash");
        var roleFromDb = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == "TEst");
        roleFromDb.Should().NotBeNull();
        roleFromDb.RoleName.Should().Be("TEst");
    }
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgres.DisposeAsync().AsTask();
    }
}
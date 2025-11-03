using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Testcontainers.PostgreSql;

namespace AuthService.Tests.Infrastructure;
public class RoleRepositoryTest : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:15")
        .WithDatabase("authtestdb")
        .WithUsername("testuser")
        .WithPassword("testpass")
        .Build();
    private AuthDbContext _context;
    private IRoleRepository _roleRepository;
    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        var options = new DbContextOptionsBuilder<AuthDbContext>()
        .UseNpgsql(_postgres.GetConnectionString()) // <-- строка подключения из контейнера
        .Options;
        _context = new AuthDbContext(options);
        await _context.Database.MigrateAsync();
        _roleRepository = new RoleRepository(_context);
    }
    [Fact]
    public async Task AddRole()
    {
        var role2 = new Role { Rolename = "Admin" };
        await _roleRepository.AddAsync(role2);
        var trackedRole = await _context.Roles.FindAsync(role2.Id);
        trackedRole.Should().NotBeNull();
        trackedRole.Rolename.Should().Be("Admin");
    }
    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _postgres.DisposeAsync().AsTask();
    }
}

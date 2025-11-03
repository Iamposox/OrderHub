using AuthService.Domain.Entities;
using AuthService.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Db;
public class AuthDbContext : DbContext
{
    public AuthDbContext(DbContextOptions options) : base(options)
    {
    }
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();

    public async Task<IDbContextTransaction> BeginTransactionAsync() => await Database.BeginTransactionAsync();
    public async Task CommitTransactionAsync() => await Database.CommitTransactionAsync();
    public async Task RollbackTransactionAsync() => await Database.RollbackTransactionAsync();
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new RoleConfiguration());
    }
}

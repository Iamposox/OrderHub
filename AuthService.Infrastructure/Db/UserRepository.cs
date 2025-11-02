using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Db;
public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _context;

    public UserRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(User entity)
    {
        await _context.Users.AddAsync(entity);
    }

    public async Task UpdateAsync(User entity)
    {
        _context.Users.Update(entity);
        await Task.CompletedTask;
    }

    public async Task<User?> GetByIdAsync(Guid id)
        => await _context.Users
        .Include(u => u.Role)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByNameAsync(string name)
        => await _context.Users
        .Include(u => u.Role)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.Username == name);

    public async Task<User?> GetByRefreshTokenAsync(string refreshToken)
        => await _context.Users
        .Include(u => u.Role)
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken);
}

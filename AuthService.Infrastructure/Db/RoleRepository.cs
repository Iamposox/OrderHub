using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Db;
public class RoleRepository : IRoleRepository
{
    private readonly AuthDbContext _context;

    public RoleRepository(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<Role?> AddAsync(Role entity)
    {
        await _context.Roles.AddAsync(entity);
        return entity;
    }

    public async Task<Role?> GetByNameAsync(string name)
        => await _context.Roles
        .AsNoTracking()
        .FirstOrDefaultAsync(u => u.RoleName == name);
}

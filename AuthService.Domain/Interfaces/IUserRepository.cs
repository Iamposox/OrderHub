using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Interfaces;
public interface IUserRepository
{
    public Task AddAsync(User entity);
    public Task UpdateAsync(User entity);
    public Task<User?> GetByIdAsync(Guid id);
    public Task<User?> GetByNameAsync(string name);
    public Task<User?> GetByRefreshTokenAsync(string refreshToken);
}

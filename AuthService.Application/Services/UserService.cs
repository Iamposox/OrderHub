using AuthService.Application.DTO;
using AuthService.Application.Extensions;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services;
public class UserService : IUserService
{
    private readonly ILogger<UserService> _logger;
    private readonly IApplicationPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepo;
    private readonly IRoleRepository _roleRepo;
    private readonly AuthDbContext _context;
    public UserService(
        ILogger<UserService> logger,
        IApplicationPasswordHasher passwordHasher,
        AuthDbContext context,
        IUserRepository userRepo,
        IRoleRepository roleRepo)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _context = context;
        _userRepo = userRepo;
        _roleRepo = roleRepo;
    }
    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func) 
    {
        _logger.LogInformation("Start transaction");
        await using var tx = await _context.BeginTransactionAsync();
        try
        {
            var result = await func();
            await _context.SaveChangesAsync();
            await tx.CommitAsync();
            _logger.LogInformation($"Executed transaction {result}");
            return result;
        }
        catch (Exception ex)
        {
            await tx.RollbackAsync();
            _logger.LogError($"Error transaction {ex.Message}");
            throw;
        }
    }
    public async Task<UserCreateDto> RegisterUserAsync(UserCreateDto userDto)
    {
        return await ExecuteInTransactionAsync(async () =>
        {
            var existingUser = await _userRepo.GetByNameAsync(userDto.Username);
            if (existingUser is not null)
                throw new InvalidOperationException($"Пользователь '{userDto.Username}' уже существует.");
            var role = await _roleRepo.GetByNameAsync(userDto.Rolename);
            if (role is null)
                role = await _roleRepo.AddAsync(new Role { RoleName = userDto.Rolename });
            var hashedPassword = _passwordHasher.HashPassword(userDto.Password);
            var user = userDto.ToEntity(hashedPassword, role);
            await _userRepo.AddAsync(user);
            return userDto;
        });
    }

    public async Task<User?> LoginUserAsync(UserLoginDto userDto)
    {
        var user = await _userRepo.GetByNameAsync(userDto.Username);
        if (user is null || !_passwordHasher.VerifyPassword(userDto.Password, user.PasswordHash))
        {
            return null;
        }
        return user;
    }
    public async Task<bool> UpdateTokenByUserAsync(User user, TokenPair tokens)
    {
        return await ExecuteInTransactionAsync(async () =>
        {
            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _userRepo.UpdateAsync(user);
            return true;
        });
    }

    public async Task<User?> GetUserByRefreshTokenAsync(string refreshToken) 
    {
        var user = await _userRepo.GetByRefreshTokenAsync(refreshToken);
        if (user is null)
        {
            return null;
        }
        return user;
    }
}

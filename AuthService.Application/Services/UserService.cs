using AuthService.Application.DTO;
using AuthService.Application.Extensions;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    public UserService(
        ILogger<UserService> logger,
        IApplicationPasswordHasher passwordHasher,
        IUnitOfWork unitOfWork,
        ITokenService tokenService)
    {
        _logger = logger;
        _passwordHasher = passwordHasher;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }
    private async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> func) 
    {
        _logger.LogInformation("Start transaction");
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var result = await func();
            await _unitOfWork.CommitTransactionAsync();
            _logger.LogInformation($"Executed transaction {result}");
            return result;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            _logger.LogError($"Error transaction {ex.Message}");
            throw;
        }
    }
    public async Task<UserCreateDto> RegisterUserAsync(UserCreateDto userDto)
    {
        return await ExecuteInTransactionAsync(async () =>
        {
            var existingUser = await _unitOfWork.Users.GetByNameAsync(userDto.Username);
            if (existingUser is not null)
                throw new InvalidOperationException($"Пользователь '{userDto.Username}' уже существует.");
            var role = await _unitOfWork.Roles.GetByNameAsync(userDto.Rolename);
            if (role is null)
                role = await _unitOfWork.Roles.AddAsync(new Role { Rolename = userDto.Rolename });
            var hashedPassword = _passwordHasher.HashPassword(userDto.Password);
            var user = userDto.ToEntity(hashedPassword, role);
            await _unitOfWork.Users.AddAsync(user);
            return userDto;
        });
    }

    public async Task<TokenPair?> LoginUserAsync(UserLoginDto userDto)
    {
        var user = await _unitOfWork.Users.GetByNameAsync(userDto.Username);
        if (user is null || !_passwordHasher.VerifyPassword(userDto.Password, user.PasswordHash))
        {
            return null;
        }
        return await GenerateAndUpdate(user);
    }
    public async Task<bool> UpdateTokenByUserAsync(User user, TokenPair tokens)
    {
        return await ExecuteInTransactionAsync(async () =>
        {
            user.RefreshToken = tokens.RefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _unitOfWork.Users.UpdateAsync(user);
            return true;
        });
    }

    public async Task<TokenPair?> GetUserByRefreshTokenAsync(string refreshToken) 
    {
        var user = await _unitOfWork.Users.GetByRefreshTokenAsync(refreshToken);
        if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
        {
            return null;
        }
        return await GenerateAndUpdate(user);
    }
    private async Task<TokenPair> GenerateAndUpdate(User user)
    {
        _logger.LogInformation($"Генерация токена и обновление пользователя {user.Username}");
        var tokens = _tokenService.GenerateTokens(user);
        var result = await UpdateTokenByUserAsync(user, tokens);
        if (result)
            return new TokenPair
            {
                AccessToken = tokens.AccessToken,
                RefreshToken = tokens.RefreshToken
            };
        else
            throw new Exception("Неизведанное");
    }
}

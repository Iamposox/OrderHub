using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Application.Services;
using AuthService.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Extensions;
public static class AuthExtension
{
    public static User ToEntity(this UserCreateDto dto, string passwordHash, Role role)
    {
        if (string.IsNullOrWhiteSpace(dto.Username))
            throw new ArgumentNullException("username is required", nameof(dto.Username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentNullException("Password hash is required", nameof(passwordHash));

        return new User
        {
            Id = Guid.NewGuid(),
            Username = dto.Username,
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public static IServiceCollection AddApplicationService(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IUserService, UserService>();
        return services;
    }
}
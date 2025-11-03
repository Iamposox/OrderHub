using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure;
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastucture(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<AuthDbContext>(options =>
            options.UseNpgsql(config.GetConnectionString("AuthConnection"),
            b => b.MigrationsAssembly("AuthService.Infrastructure")));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
        services.AddScoped<IApplicationPasswordHasher, PasswordHasherAdapter>();
        return services;
    }
}

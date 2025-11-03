using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Application.Settings;
using AuthService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Services;
public class TokenService : ITokenService
{
    private readonly TokenSettings _tokenSettings;
    private readonly ILogger<TokenService> _logger;
    public TokenService(IOptions<TokenSettings> tokenSettings, ILogger<TokenService> logger)
    {
        _tokenSettings = tokenSettings.Value;
        _logger = logger;
    }

    public TokenPair GenerateTokens(User user)
    {
        try
        {
            var jwtKey = _tokenSettings.Key;
            var issuer = _tokenSettings.Issuer;
            var audience = _tokenSettings.Audience;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Username),
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.Rolename)
            };

            var accessToken = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddMinutes(15),
                signingCredentials: creds);

            var refreshToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");

            return new TokenPair
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(accessToken),
                RefreshToken = refreshToken
            };
        }
        catch (Exception ex)
        {
            _logger.LogError($"Произошла ошибка при генерации токена: {ex.Message}");
            throw;
        }
    }
}

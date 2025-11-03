using AuthService.Application.Services;
using AuthService.Application.Settings;
using AuthService.Domain.Entities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace AuthService.Tests.Application;
public class TokenServiceTest : IAsyncLifetime
{
    private TokenService _tokenService;

    public async Task InitializeAsync()
    {
        var options = Options.Create(new TokenSettings
        {
            Key = "Lalala-lalala-lalalala-lala-lalala-la",
            Audience = "LA",
            Issuer = "LAis"
        });

        var logger = NullLogger<TokenService>.Instance;

        _tokenService = new TokenService(options, logger);
    }
    public async Task DisposeAsync()
    {
    }

    [Fact]
    public async Task GenerateTokensDone()
    {
        var user = new User 
        {
            Id = Guid.NewGuid(),
            Username = "Danila",
            PasswordHash = "Hash",
            Role = new Role
            {
                Id = Guid.NewGuid(),
                RoleName = "Test"
            }
        };
        var result = _tokenService.GenerateTokens(user);

        result.Should().NotBeNull();
        
    }
}

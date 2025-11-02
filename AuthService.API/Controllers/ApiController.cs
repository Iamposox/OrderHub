using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Interfaces;
using AuthService.Infrastructure.Db;
using AuthService.Infrastructure.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{

    private readonly ILogger<ApiController> _logger;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    public ApiController(
        ILogger<ApiController> logger,
        IUserService userService,
        ITokenService tokenService)
    {
        _logger = logger;
        _userService = userService;
        _tokenService = tokenService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(UserCreateDto model)
    {
        _logger.LogInformation($"Получили запрос регистрации {model.Username}");
        try
        {
            var hashedPassword = await _userService.RegisterUserAsync(model);
            _logger.LogInformation($"Завершили регистрацию");
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка регистрации {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(UserLoginDto model)
    {
        _logger.LogInformation($"Получили запрос авторизации {model}");
        try
        {
            var user = await _userService.LoginUserAsync(model);
            if (user is null)
            {
                _logger.LogError($"Пользователь не найден {model.Username}");
                return Unauthorized();
            }
            var result = await GenerateAndUpdate(user);
            _logger.LogInformation($"Завершили авторизацию");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка авторизации {ex.Message}");
            return BadRequest(ex.Message);
        }
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto model)
    {
        _logger.LogInformation($"Получили запрос рефреша {model}");
        try
        {
            var user = await _userService.GetUserByRefreshTokenAsync(model.RefreshToken);
            if (user is null || user.RefreshTokenExpiry < DateTime.UtcNow)
            {
                _logger.LogError($"Пользователь не авторизован/ не найден {model}");
                return Unauthorized();
            }
            var result = await GenerateAndUpdate(user);
            _logger.LogInformation($"Завершили рефреш {user.Username} - {result}");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка рефреша {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
    private async Task<IActionResult> GenerateAndUpdate(User user)
    {
        _logger.LogInformation($"Генерация токена и обновление пользователя {user.Username}");
        var tokens = _tokenService.GenerateTokens(user);
        var result = await _userService.UpdateTokenByUserAsync(user, tokens);
        if (result)
            return Ok(new
            {
                tokens.AccessToken,
                tokens.RefreshToken
            });
        else
            return BadRequest($"Неизведанное\r\n{nameof(Login) + " - " + nameof(ApiController)}");
    }
}

using AuthService.Application.DTO;
using AuthService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.API.Controllers;

[ApiController]
[Route("[controller]")]
public class ApiController : ControllerBase
{

    private readonly ILogger<ApiController> _logger;
    private readonly IUserService _userService;
    public ApiController(
        ILogger<ApiController> logger,
        IUserService userService)
    {
        _logger = logger;
        _userService = userService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync(UserCreateDto model)
    {
        _logger.LogInformation($"Получили запрос регистрации {model.Username}");
        try
        {
            var userDto = await _userService.RegisterUserAsync(model);
            if(userDto is null)
            {
                _logger.LogError($"Регмстрация неуспешна {model.Username}");
                return BadRequest(model);
            }
            _logger.LogInformation($"Завершили регистрацию");
            return Ok(userDto);
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
                return Unauthorized(model);
            }
            _logger.LogInformation($"Завершили авторизацию");
            return Ok(user);
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
            if (user is null)
            {
                _logger.LogError($"Пользователь не авторизован/ не найден {model}");
                return Unauthorized(model);
            }
            _logger.LogInformation($"Завершили рефреш");
            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ошибка рефреша {ex.Message}");
            return BadRequest(ex.Message);
        }
    }
}

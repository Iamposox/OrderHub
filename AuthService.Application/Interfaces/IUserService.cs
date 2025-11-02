using AuthService.Application.DTO;
using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.Interfaces;
public interface IUserService
{
    public Task<UserCreateDto> RegisterUserAsync(UserCreateDto userDto);
    public Task<User?> LoginUserAsync(UserLoginDto userDto);
    public Task<User?> GetUserByRefreshTokenAsync(string token);
    public Task<bool> UpdateTokenByUserAsync(User userDto, TokenPair tokens);
}

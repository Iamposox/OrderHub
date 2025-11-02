using AuthService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTO;
public record UserCreateDto(string Username, string Password, string Rolename);
public record UserLoginDto(string Username, string Password);
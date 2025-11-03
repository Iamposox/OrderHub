using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Domain.Entities;
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = default!;
    public string PasswordHash { get; set; } = default!;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry {  get; set; }
    public Guid RoleId { get; set; }
    public Role Role { get; set; }
}

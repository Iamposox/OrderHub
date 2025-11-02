using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Application.DTO;
public class TokenPair
{
    public string AccessToken { get; set; } = default!;
    public string RefreshToken { get; set; } = default!;
}
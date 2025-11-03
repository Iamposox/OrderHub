using AuthService.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuthService.Infrastructure.Security;
public class PasswordHasherAdapter : IApplicationPasswordHasher
{
    private readonly IPasswordHasher<object> _hasher;
    public PasswordHasherAdapter(IPasswordHasher<object> hasher) => _hasher = hasher;
    public string HashPassword(string password)
    {
        if(string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException("password");
        return _hasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hash)
    {
        if(string.IsNullOrWhiteSpace(password))
            throw new ArgumentNullException("password");
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentNullException("hash");
        return _hasher.VerifyHashedPassword(null!, hash, password) != PasswordVerificationResult.Failed;
    }
}

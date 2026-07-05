using Btr.Application.Abstractions.Security;
using Btr.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Btr.Infrastructure.Security;

public sealed class PasswordHasherAdapter : IPasswordHasher
{
    private readonly PasswordHasher<User> _passwordHasher = new();

    public string HashPassword(string password)
    {
        var user = new User();
        return _passwordHasher.HashPassword(user, password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        var user = new User();
        var result = _passwordHasher.VerifyHashedPassword(user, passwordHash, password);
        return result is PasswordVerificationResult.Success or PasswordVerificationResult.SuccessRehashNeeded;
    }
}

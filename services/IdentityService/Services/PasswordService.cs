using Microsoft.AspNetCore.Identity;

namespace BookNook.IdentityService.Services;

public class PasswordService
{
    private readonly PasswordHasher<string> _passwordHasher = new();

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(
            "BookNook",
            password
        );
    }

    public bool VerifyPassword(string hashedPassword, string password)
    {
        var result = _passwordHasher.VerifyHashedPassword(
            "BookNook",
            hashedPassword,
            password
        );

        return result != PasswordVerificationResult.Failed;
    }
}

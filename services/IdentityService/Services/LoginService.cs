using BookNook.IdentityService.Data;
using BookNook.IdentityService.Models;
using BookNook.IdentityService.Services;
using Microsoft.EntityFrameworkCore;

namespace BookNook.IdentityService.Services;

public class LoginService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordService _passwordService;

    public LoginService(
        ApplicationDbContext context,
        PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<User?> LoginAsync(string email, string password)
    {
        var normalizedEmail = email.Trim().ToLowerInvariant();

        var user = await _context.Users
            .FirstOrDefaultAsync(user => user.Email == normalizedEmail);

        if (user == null || !user.IsActive)
        {
            return null;
        }

        var passwordIsValid = _passwordService.VerifyPassword(
            user.PasswordHash,
            password
        );

        if (!passwordIsValid)
        {
            return null;
        }

        return user;
    }
}

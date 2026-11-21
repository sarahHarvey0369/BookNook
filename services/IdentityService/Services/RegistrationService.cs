using BookNook.IdentityService.Data;
using BookNook.IdentityService.DTOs;
using BookNook.IdentityService.Models;
using Microsoft.EntityFrameworkCore;

namespace BookNook.IdentityService.Services;

public class RegistrationService
{
    private readonly ApplicationDbContext _context;
    private readonly PasswordService _passwordService;

    public RegistrationService(
        ApplicationDbContext context,
        PasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public async Task<bool> RegisterAsync(RegisterRequest request)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();

        var emailExists = await _context.Users
            .AnyAsync(user => user.Email == normalizedEmail);

        if (emailExists)
        {
            return false;
        }

        var user = new User
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            Email = normalizedEmail,
            PasswordHash = _passwordService.HashPassword(request.Password)
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return true;
    }
}

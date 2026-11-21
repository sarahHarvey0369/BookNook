using BookNook.IdentityService.DTOs;
using BookNook.IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegistrationService _registrationService;

    public AuthController(RegistrationService registrationService)
    {
        _registrationService = registrationService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        var registered = await _registrationService.RegisterAsync(request);

        if (!registered)
        {
            return Conflict(new
            {
                message = "An account with this email already exists."
            });
        }

        return Ok(new
        {
            message = "Account created successfully."
        });
    }
}

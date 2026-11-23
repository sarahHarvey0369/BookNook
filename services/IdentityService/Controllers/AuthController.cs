using BookNook.IdentityService.DTOs;
using BookNook.IdentityService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly RegistrationService _registrationService;
    private readonly LoginService _loginService;
    private readonly JwtService _jwtService;

    public AuthController(
        RegistrationService registrationService,
        LoginService loginService,
        JwtService jwtService)
    {
        _registrationService = registrationService;
        _loginService = loginService;
        _jwtService = jwtService;
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

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var user = await _loginService.LoginAsync(
            request.Email,
            request.Password
        );

        if (user == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var response = _jwtService.GenerateToken(user);

        return Ok(response);
    }
}

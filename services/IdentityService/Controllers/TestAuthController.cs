using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BookNook.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TestAuthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        var email = User.FindFirstValue("email");

        return Ok(new
        {
            message = "You are successfully authenticated!",
            userId,
            email
        });
    }
}

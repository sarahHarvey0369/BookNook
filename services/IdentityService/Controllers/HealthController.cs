using Microsoft.AspNetCore.Mvc;

namespace BookNook.IdentityService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok(new
        {
            service = "BookNook Identity Service",
            status = "Healthy",
            timestamp = DateTime.UtcNow
        });
    }
}

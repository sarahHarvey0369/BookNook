using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using BookNook.ReadingStatisticsService.DTOs;
using BookNook.ReadingStatisticsService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.ReadingStatisticsService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StatisticsController(StatisticsService statisticsService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ReadingStatisticsResponse>> GetStatistics()
    {
        var accessToken = GetAccessToken();

        if (accessToken == null)
        {
            return Unauthorized();
        }

        var statistics = await statisticsService.GetStatisticsAsync(accessToken);

        return Ok(statistics);
    }

    private string? GetAccessToken()
    {
        var authorizationHeader = Request.Headers.Authorization.ToString();

        if (!AuthenticationHeaderValue.TryParse(authorizationHeader, out var header) ||
            !string.Equals(header.Scheme, "Bearer", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        return header.Parameter;
    }
}
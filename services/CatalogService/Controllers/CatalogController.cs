using BookNook.CatalogService.DTOs;
using BookNook.CatalogService.Models;
using BookNook.CatalogService.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookNook.CatalogService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CatalogController : ControllerBase
{
    private readonly GoogleBooksService _googleBooksService;
    private readonly OpenLibraryService _openLibraryService;
    private readonly ILogger<CatalogController> _logger;

    public CatalogController(
        GoogleBooksService googleBooksService,
        OpenLibraryService openLibraryService,
        ILogger<CatalogController> logger)
    {
        _googleBooksService = googleBooksService;
        _openLibraryService = openLibraryService;
        _logger = logger;
    }

    [HttpGet("search")]
    public async Task<ActionResult<BookSearchResponse>> Search(
        [FromQuery] string query,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return BadRequest("A search query is required.");
        }

        if (page < 1)
        {
            return BadRequest("Page must be greater than 0.");
        }

        // Maximum supported page size by Google Books is 40.
        if (pageSize < 1 || pageSize > 40)
        {
            return BadRequest("Page size must be between 1 and 40.");
        }

        try
        {
            CatalogSearchResult searchResult = new();

            try
            {
                searchResult = await _googleBooksService.SearchAsync(query, page, pageSize);
            }
            catch (HttpRequestException exception)
            {
                _logger.LogWarning(
                    exception,
                    "Google Books search failed. Falling back to Open Library.");
            }

            if (searchResult.Books.Count == 0)
            {
                searchResult = await _openLibraryService.SearchAsync(query, page, pageSize);
            }

            var result = new BookSearchResponse
            {
                Query = query,
                TotalResults = searchResult.TotalResults,
                Page = page,
                PageSize = pageSize,
                TotalPages = (int)Math.Ceiling(
                    (double)searchResult.TotalResults / pageSize),
                Books = searchResult.Books
            };

            return Ok(result);
        }
        catch (HttpRequestException exception)
        {
            _logger.LogError(exception, "Book catalog search failed.");

            return StatusCode(
                StatusCodes.Status503ServiceUnavailable,
                "Book search is temporarily unavailable. Please try again later.");
        }
    }
}

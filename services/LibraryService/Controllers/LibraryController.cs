using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BookNook.LibraryService.Data;
using BookNook.LibraryService.DTOs;
using BookNook.LibraryService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookNook.LibraryService.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LibraryController(LibraryDbContext context) : ControllerBase
{
    private Guid? GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return null;
        }

        return userId;
    }

    [HttpPost]
    public async Task<IActionResult> AddBook(AddBookRequest request)
    {
        // Get the authenticated user's ID from the JWT token.
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        // Check whether this book is already in the user's library.
        var bookExists = await context.LibraryBooks.AnyAsync(book =>
            book.UserId == userId &&
            book.ExternalBookId == request.ExternalBookId &&
            book.CatalogSource == request.CatalogSource);

        if (bookExists)
        {
            return Conflict("This book is already in your library.");
        }

        // Create the user's library entry.
        var libraryBook = new LibraryBook
        {
            UserId = userId.Value,
            ExternalBookId = request.ExternalBookId,
            CatalogSource = request.CatalogSource,
            Title = request.Title,
            Authors = request.Authors,
            Description = request.Description,
            ISBN = request.ISBN,
            CoverImageUrl = request.CoverImageUrl,
            Publisher = request.Publisher,
            PublishedYear = request.PublishedYear,
            PageCount = request.PageCount,
            Status = ReadingStatus.WantToRead,
            AddedAt = DateTime.UtcNow
        };

        context.LibraryBooks.Add(libraryBook);
        await context.SaveChangesAsync();

        return Ok(LibraryBookResponse.FromEntity(libraryBook));
    }

    [HttpGet]
    public async Task<ActionResult<List<LibraryBookResponse>>> GetLibrary()
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var books = await context.LibraryBooks
            .Where(book => book.UserId == userId.Value)
            .OrderByDescending(book => book.AddedAt)
            .ToListAsync();

        var response = books
            .Select(LibraryBookResponse.FromEntity)
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<LibraryBookResponse>> GetBookById(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var book = await context.LibraryBooks
            .FirstOrDefaultAsync(book =>
                book.Id == id && book.UserId == userId.Value);

        if (book == null)
        {
            return NotFound("Book not found in your library.");
        }

        return Ok(LibraryBookResponse.FromEntity(book));
    }

    [HttpPut("{id:guid}/progress")]
    public async Task<IActionResult> UpdateReadingProgress(
    Guid id,
    UpdateReadingProgressRequest request)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var book = await context.LibraryBooks
            .FirstOrDefaultAsync(book =>
                book.Id == id && book.UserId == userId.Value);

        if (book == null)
        {
            return NotFound("Book not found in your library.");
        }

        if (request.CurrentPage < 0 ||
            (book.PageCount.HasValue &&
             request.CurrentPage > book.PageCount))
        {
            return BadRequest("Current page is outside the book's page range.");
        }

        if (!Enum.IsDefined(request.Status))
        {
            return BadRequest("Invalid reading status.");
        }

        book.Status = request.Status;
        book.CurrentPage = request.CurrentPage;
        book.UpdatedAt = DateTime.UtcNow;

        if (request.Status == ReadingStatus.CurrentlyReading &&
            book.StartedAt == null)
        {
            book.StartedAt = DateTime.UtcNow;
        }

        if (request.Status == ReadingStatus.Read)
        {
            book.FinishedAt = DateTime.UtcNow;

            if (book.PageCount.HasValue)
            {
                book.CurrentPage = book.PageCount;
            }
        }
        else
        {
            book.FinishedAt = null;
        }

        await context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteBook(Guid id)
    {
        var userId = GetCurrentUserId();

        if (userId == null)
        {
            return Unauthorized();
        }

        var book = await context.LibraryBooks
            .FirstOrDefaultAsync(book =>
                book.Id == id && book.UserId == userId.Value);

        if (book == null)
        {
            return NotFound("Book not found in your library.");
        }

        context.LibraryBooks.Remove(book);

        await context.SaveChangesAsync();

        return NoContent();
    }
}
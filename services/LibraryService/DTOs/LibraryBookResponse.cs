using BookNook.LibraryService.Models;

namespace BookNook.LibraryService.DTOs;

public class LibraryBookResponse
{
    public Guid Id { get; set; }
    public string ExternalBookId { get; set; } = string.Empty;
    public string CatalogSource { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public int? PageCount { get; set; }
    public ReadingStatus Status { get; set; }
    public int? CurrentPage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime AddedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public static LibraryBookResponse FromEntity(LibraryBook book)
    {
        return new LibraryBookResponse
        {
            Id = book.Id,
            ExternalBookId = book.ExternalBookId,
            CatalogSource = book.CatalogSource,
            Title = book.Title,
            Authors = book.Authors,
            Description = book.Description,
            ISBN = book.ISBN,
            CoverImageUrl = book.CoverImageUrl,
            Publisher = book.Publisher,
            PublishedYear = book.PublishedYear,
            PageCount = book.PageCount,
            Status = book.Status,
            CurrentPage = book.CurrentPage,
            StartedAt = book.StartedAt,
            FinishedAt = book.FinishedAt,
            AddedAt = book.AddedAt,
            UpdatedAt = book.UpdatedAt
        };
    }
}
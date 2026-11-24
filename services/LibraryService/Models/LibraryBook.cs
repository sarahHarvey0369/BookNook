namespace BookNook.LibraryService.Models;

public class LibraryBook
{
    public Guid Id { get; set; } = Guid.NewGuid();

    // The user who owns this library entry.
    public Guid UserId { get; set; }

    // Identifies the book in the external catalog.
    public string ExternalBookId { get; set; } = string.Empty;
    public string CatalogSource { get; set; } = string.Empty;

    // Book information saved from the catalog.
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public int? PageCount { get; set; }

    // Reading information.
    public ReadingStatus Status { get; set; } = ReadingStatus.WantToRead;
    public int? CurrentPage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }

    // Record timestamps.
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
namespace BookNook.CatalogService.Models;

public class Book
{
    public string ExternalId { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public string? Description { get; set; }
    public string? ISBN { get; set; }
    public string? CoverImageUrl { get; set; }
    public string? Publisher { get; set; }
    public int? PublishedYear { get; set; }
    public int? PageCount { get; set; }
}

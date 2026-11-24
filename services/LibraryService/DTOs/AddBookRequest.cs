using System.ComponentModel.DataAnnotations;

namespace BookNook.LibraryService.DTOs;

public class AddBookRequest
{
    [Required]
    [StringLength(100)]
    public string ExternalBookId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string CatalogSource { get; set; } = string.Empty;

    [Required]
    [StringLength(500)]
    public string Title { get; set; } = string.Empty;

    public List<string> Authors { get; set; } = [];

    public string? Description { get; set; }

    [StringLength(20)]
    public string? ISBN { get; set; }

    public string? CoverImageUrl { get; set; }

    [StringLength(255)]
    public string? Publisher { get; set; }

    public int? PublishedYear { get; set; }

    public int? PageCount { get; set; }
}
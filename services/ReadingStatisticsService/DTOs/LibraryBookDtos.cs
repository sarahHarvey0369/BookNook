namespace BookNook.ReadingStatisticsService.DTOs;

public class LibraryBookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public List<string> Authors { get; set; } = [];
    public int? PageCount { get; set; }
    public ReadingStatusDto Status { get; set; }
    public int? CurrentPage { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public DateTime AddedAt { get; set; }
}
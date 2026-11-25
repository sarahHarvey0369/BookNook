namespace BookNook.ReadingStatisticsService.DTOs;

public class ReadingStatisticsResponse
{
    public int TotalBooks { get; set; }
    public int WantToRead { get; set; }
    public int CurrentlyReading { get; set; }
    public int Read { get; set; }
    public int OnHold { get; set; }
    public int TotalPagesRead { get; set; }
    public int BooksFinishedThisYear { get; set; }
}
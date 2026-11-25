using BookNook.ReadingStatisticsService.DTOs;

namespace BookNook.ReadingStatisticsService.Services;

public class StatisticsService(LibraryServiceClient libraryServiceClient)
{
    public async Task<ReadingStatisticsResponse> GetStatisticsAsync(string accessToken)
    {
        var books = await libraryServiceClient.GetLibraryAsync(accessToken);
        var currentYear = DateTime.UtcNow.Year;

        return new ReadingStatisticsResponse
        {
            TotalBooks = books.Count,
            WantToRead = books.Count(book =>
                book.Status == ReadingStatusDto.WantToRead),
            CurrentlyReading = books.Count(book =>
                book.Status == ReadingStatusDto.CurrentlyReading),
            Read = books.Count(book =>
                book.Status == ReadingStatusDto.Read),
            OnHold = books.Count(book =>
                book.Status == ReadingStatusDto.OnHold),
            TotalPagesRead = books
                .Where(book => book.Status == ReadingStatusDto.Read)
                .Sum(book => book.PageCount ?? 0),
            BooksFinishedThisYear = books.Count(book =>
                book.Status == ReadingStatusDto.Read &&
                book.FinishedAt.HasValue &&
                book.FinishedAt.Value.Year == currentYear)
        };
    }
}
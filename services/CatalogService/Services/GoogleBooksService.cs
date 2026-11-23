using System.Text.Json;
using BookNook.CatalogService.Models;

namespace BookNook.CatalogService.Services;

public class GoogleBooksService
{
    private readonly HttpClient _httpClient;

    public GoogleBooksService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CatalogSearchResult> SearchAsync(
        string query,
        int page,
        int pageSize)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var startIndex = (page - 1) * pageSize;
        var url = $"https://www.googleapis.com/books/v1/volumes?q={encodedQuery}&maxResults={pageSize}&startIndex={startIndex}";
        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        var result = new CatalogSearchResult
        {
            TotalResults = document.RootElement.TryGetProperty(
                "totalItems", out var total)
                    ? total.GetInt32()
                    : 0
        };

        var books = new List<Book>();

        if (!document.RootElement.TryGetProperty("items", out var items))
        {
            return result;
        }

        foreach (var item in items.EnumerateArray())
        {
            var volumeInfo = item.GetProperty("volumeInfo");

            var book = new Book
            {
                ExternalId = item.GetProperty("id").GetString() ?? string.Empty,
                Source = "GoogleBooks",
                Title = volumeInfo.TryGetProperty("title", out var title)
                    ? title.GetString() ?? "Unknown Title"
                    : "Unknown Title",
                Description = volumeInfo.TryGetProperty("description", out var description)
                    ? description.GetString()
                    : null,
                Publisher = volumeInfo.TryGetProperty("publisher", out var publisher)
                    ? publisher.GetString()
                    : null,
                PublishedYear = GetPublishedYear(volumeInfo),
                PageCount = volumeInfo.TryGetProperty("pageCount", out var pageCount)
                    ? pageCount.GetInt32()
                    : null,
                ISBN = GetIsbn(volumeInfo),
                CoverImageUrl = GetCoverImage(volumeInfo),
                Authors = GetAuthors(volumeInfo)
            };

            books.Add(book);
        }

        result.Books = books;
        return result;
    }

    private static int? GetPublishedYear(JsonElement volumeInfo)
    {
        if (!volumeInfo.TryGetProperty("publishedDate", out var publishedDate))
        {
            return null;
        }

        var date = publishedDate.GetString();

        if (date != null && date.Length >= 4 &&
            int.TryParse(date[..4], out var year))
        {
            return year;
        }

        return null;
    }

    private static string? GetIsbn(JsonElement volumeInfo)
    {
        if (!volumeInfo.TryGetProperty("industryIdentifiers", out var identifiers))
        {
            return null;
        }

        foreach (var identifier in identifiers.EnumerateArray())
        {
            if (identifier.TryGetProperty("type", out var type) &&
                type.GetString() is "ISBN_13" or "ISBN_10" &&
                identifier.TryGetProperty("identifier", out var value))
            {
                return value.GetString();
            }
        }

        return null;
    }

    private static string? GetCoverImage(JsonElement volumeInfo)
    {
        if (!volumeInfo.TryGetProperty("imageLinks", out var imageLinks))
        {
            return null;
        }

        return imageLinks.TryGetProperty("thumbnail", out var thumbnail)
            ? thumbnail.GetString()
            : null;
    }

    private static List<string> GetAuthors(JsonElement volumeInfo)
    {
        var authors = new List<string>();

        if (volumeInfo.TryGetProperty("authors", out var authorArray))
        {
            foreach (var author in authorArray.EnumerateArray())
            {
                var name = author.GetString();

                if (!string.IsNullOrWhiteSpace(name))
                {
                    authors.Add(name);
                }
            }
        }

        return authors;
    }
}

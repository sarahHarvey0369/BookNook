using System.Text.Json;
using BookNook.CatalogService.Models;

namespace BookNook.CatalogService.Services;

public class OpenLibraryService
{
    private readonly HttpClient _httpClient;

    public OpenLibraryService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<CatalogSearchResult> SearchAsync(
        string query,
        int page,
        int pageSize)
    {
        var encodedQuery = Uri.EscapeDataString(query);
        var offset = (page - 1) * pageSize;
        var url = $"https://openlibrary.org/search.json?q={encodedQuery}&limit={pageSize}&offset={offset}";
        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync();
        using var document = await JsonDocument.ParseAsync(stream);

        var result = new CatalogSearchResult
        {
            TotalResults = document.RootElement.TryGetProperty(
                "numFound", out var total)
                    ? total.GetInt32()
                    : 0
        };

        var books = new List<Book>();

        if (!document.RootElement.TryGetProperty("docs", out var docs))
        {
            return result;
        }

        foreach (var item in docs.EnumerateArray())
        {
            var book = new Book
            {
                ExternalId = GetString(item, "key") ?? string.Empty,
                Source = "OpenLibrary",
                Title = GetString(item, "title") ?? "Unknown Title",
                Authors = GetStringArray(item, "author_name"),
                PublishedYear = GetFirstInt(item, "first_publish_year"),
                ISBN = GetFirstString(item, "isbn"),
                CoverImageUrl = GetCoverImage(item),
                Publisher = GetFirstString(item, "publisher")
            };

            books.Add(book);
        }

        result.Books = books;
        return result;
    }

    private static string? GetString(JsonElement element, string property)
    {
        return element.TryGetProperty(property, out var value)
            ? value.GetString()
            : null;
    }

    private static List<string> GetStringArray(JsonElement element, string property)
    {
        var values = new List<string>();

        if (element.TryGetProperty(property, out var array))
        {
            foreach (var item in array.EnumerateArray())
            {
                var value = item.GetString();

                if (!string.IsNullOrWhiteSpace(value))
                {
                    values.Add(value);
                }
            }
        }

        return values;
    }

    private static string? GetFirstString(JsonElement element, string property)
    {
        if (element.TryGetProperty(property, out var array) &&
            array.ValueKind == JsonValueKind.Array &&
            array.GetArrayLength() > 0)
        {
            return array[0].GetString();
        }

        return null;
    }

    private static int? GetFirstInt(JsonElement element, string property)
    {
        if (element.TryGetProperty(property, out var value) &&
            value.ValueKind == JsonValueKind.Number &&
            value.TryGetInt32(out var number))
        {
            return number;
        }

        return null;
    }

    private static string? GetCoverImage(JsonElement item)
    {
        if (item.TryGetProperty("cover_i", out var coverId) &&
            coverId.ValueKind == JsonValueKind.Number)
        {
            return $"https://covers.openlibrary.org/b/id/{coverId.GetInt32()}-M.jpg";
        }

        return null;
    }
}

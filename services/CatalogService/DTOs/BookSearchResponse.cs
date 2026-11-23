using BookNook.CatalogService.Models;

namespace BookNook.CatalogService.DTOs;

public class BookSearchResponse
{
    public string Query { get; set; } = string.Empty;
    public int TotalResults { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public List<Book> Books { get; set; } = [];
}

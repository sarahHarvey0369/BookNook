namespace BookNook.CatalogService.Models;

public class CatalogSearchResult
{
    public List<Book> Books { get; set; } = [];
    public int TotalResults { get; set; }
}

namespace Catalog.Application.Models;

public class ProductSearchDocument
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public decimal AverageStars { get; set; }
    public int ReviewCount { get; set; }
}
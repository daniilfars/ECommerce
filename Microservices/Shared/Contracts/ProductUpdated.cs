namespace Shared.Contracts;

public interface ProductUpdated
{
    int Id { get; }
    string Name { get; }
    string? ImageUrl { get; }
    decimal Price { get; }
    int StockQuantity { get; }
}
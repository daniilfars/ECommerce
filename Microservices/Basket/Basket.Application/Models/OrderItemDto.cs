namespace Basket.Application.Models;

public sealed record OrderItemDto(int ProductId, string ProductName, decimal PriceAmount, string PriceCurrency, int Quantity, string? ImageUrl);
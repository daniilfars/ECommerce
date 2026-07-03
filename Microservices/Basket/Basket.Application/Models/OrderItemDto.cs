namespace Basket.Application.Models;

public sealed record OrderItemDto(int ProductId, string ProductName, decimal Price, int Quantity, string? ImageUrl);
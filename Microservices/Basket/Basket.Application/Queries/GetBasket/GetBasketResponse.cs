namespace Basket.Application.Queries.GetBasket;

public sealed record GetBasketResponse(Guid UserId, List<BasketItemDto> Items, decimal TotalAmount);

public sealed record BasketItemDto(int ProductId, string ProductName, decimal Price, int Quantity, decimal TotalPrice, string? imageUrl);
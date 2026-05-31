namespace Modules.Basket.Application.Queries.GetBasket;

public sealed record GetBasketResponse(Guid UserId, List<BasketItemDto> Items, decimal TotalAmount);

public sealed record BasketItemDto(int ProductId, string ProductName, decimal PriceAmount,string PriceCurrency, int Quantity, decimal TotalPrice);
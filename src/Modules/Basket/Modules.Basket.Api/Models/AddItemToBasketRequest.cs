namespace Modules.Basket.Api.Models;

public sealed record AddItemToBasketRequest(int ProductId, int Quantity);
namespace Basket.Application.Models;

public sealed record CreateOrderCommand(Guid UserId, string ShippingAddress, List<OrderItemDto> Items);
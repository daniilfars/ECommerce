namespace Ordering.Application.Commands.CreateOrder;

public sealed record CreateOrderResponse(Guid Id, Guid UserId, string ShippingAddress, string Status, decimal TotalAmount);
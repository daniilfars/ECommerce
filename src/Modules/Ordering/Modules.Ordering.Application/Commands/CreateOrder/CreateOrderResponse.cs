namespace Modules.Ordering.Application.Commands.CreateOrder;

public sealed record CreateOrderResponse(int Id, Guid UserId, string ShippingAddress, string Status, decimal TotalAmount);
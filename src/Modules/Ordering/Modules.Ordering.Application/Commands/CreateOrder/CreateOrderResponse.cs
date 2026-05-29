namespace Modules.Ordering.Application.Commands.CreateOrder;

public sealed record CreateOrderResponse(int Id, int UserId, string ShippingAddress, string Status, decimal TotalAmount);
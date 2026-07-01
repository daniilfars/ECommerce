namespace Ordering.Application.Queries.GetOrderById;

public sealed record GetOrderByIdResponse(Guid UserId, string Status, string ShippingAddress, decimal TotalAmount, string? PaymentId, List<OrderItemDto> Items);

public sealed record OrderItemDto(int Id, int ProductId, string ProductName, decimal PriceAmount, string PriceCurrency, int Quantity, decimal TotalPrice, string? ImageUrl);
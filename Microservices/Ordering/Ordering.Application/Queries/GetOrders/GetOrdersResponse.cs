namespace Ordering.Application.Queries.GetOrders;

public sealed record GetOrdersResponse(List<OrderDto> Orders, int TotalCount, int Page, int PageSize);

public sealed record OrderDto(int Id, Guid UserId, string ShippingAddress, string Status, decimal TotalAmount, int ItemsCount);
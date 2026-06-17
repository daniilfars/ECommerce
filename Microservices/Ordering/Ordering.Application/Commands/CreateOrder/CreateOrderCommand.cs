using MediatR;
using Shared.Domain;

namespace Ordering.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid UserId, string ShippingAddress, List<OrderItemDetailDto> Items) : IRequest<Result<CreateOrderResponse>>;

public sealed record OrderItemDetailDto(int ProductId, string ProductName, decimal PriceAmount, string PriceCurrency, int Quantity, string? ImageUrl);
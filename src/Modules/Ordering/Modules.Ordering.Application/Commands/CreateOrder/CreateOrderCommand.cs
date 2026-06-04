using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Commands.CreateOrder;

public sealed record CreateOrderCommand(Guid UserId, string ShippingAddress, List<OrderItemDto> Items) : IRequest<Result<CreateOrderResponse>>;

public sealed record OrderItemDto(int ProductId, string ProductName, decimal PriceAmount, string PriceCurrency, int Quantity, string? ImageUrl);
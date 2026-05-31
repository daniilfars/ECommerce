using MediatR;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.CheckoutBasket;

public sealed record CheckoutBasketCommand(Guid UserId, string ShippingAddress) : IRequest<Result>;
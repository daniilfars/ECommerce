using MediatR;
using Basket.Application.Queries.GetBasket;
using Shared.Domain;

namespace Basket.Application.Commands.UpdateBasketItemQuantity;

public sealed record UpdateBasketItemQuantityCommand(Guid UserId, int ProductId, int Quantity) : IRequest<Result<GetBasketResponse>>;
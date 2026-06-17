using MediatR;
using Basket.Application.Queries.GetBasket;
using Shared.Domain;

namespace Basket.Application.Commands.RemoveItemFromBasket;

public sealed record class RemoveItemFromBasketCommand(Guid UserId, int ProductId) : IRequest<Result<GetBasketResponse>>;
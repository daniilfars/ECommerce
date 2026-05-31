using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.RemoveItemFromBasket;

public sealed record class RemoveItemFromBasketCommand(Guid UserId, int ProductId) : IRequest<Result<GetBasketResponse>>;
using MediatR;
using Modules.Basket.Application.Queries.GetBasket;
using Shared.Domain;

namespace Modules.Basket.Application.Commands.AddItemToBasket;

public sealed record class AddItemToBasketCommand(Guid UserId, int ProductId, int Quantity) : IRequest<Result<GetBasketResponse>>;
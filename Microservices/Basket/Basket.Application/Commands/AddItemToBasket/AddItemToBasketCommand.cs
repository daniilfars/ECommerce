using MediatR;
using Basket.Application.Queries.GetBasket;
using Shared.Domain;

namespace Basket.Application.Commands.AddItemToBasket;

public sealed record class AddItemToBasketCommand(Guid UserId, int ProductId, int Quantity) : IRequest<Result<GetBasketResponse>>;
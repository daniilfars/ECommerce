using Shared.Domain;
using MediatR;

namespace Basket.Application.Queries.GetBasket;

public sealed record GetBasketQuery(Guid UserId) : IRequest<Result<GetBasketResponse>>;
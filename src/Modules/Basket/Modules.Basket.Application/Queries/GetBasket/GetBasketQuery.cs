using Shared.Domain;
using MediatR;

namespace Modules.Basket.Application.Queries.GetBasket;

public sealed record GetBasketQuery(Guid UserId) : IRequest<Result<GetBasketResponse>>;
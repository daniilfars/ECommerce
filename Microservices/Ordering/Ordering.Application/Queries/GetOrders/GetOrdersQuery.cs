using MediatR;
using Shared.Domain;

namespace Ordering.Application.Queries.GetOrders;

public sealed record class GetOrdersQuery(Guid UserId, int Page = 1, int PageSize = 10, bool All = false) : IRequest<Result<GetOrdersResponse>>;

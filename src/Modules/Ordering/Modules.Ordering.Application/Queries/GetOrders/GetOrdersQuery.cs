using MediatR;
using Shared.Domain;

namespace Modules.Ordering.Application.Queries.GetOrders;

public sealed record class GetOrdersQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<Result<GetOrdersResponse>>;

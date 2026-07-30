using MediatR;
using Shared.Domain;

namespace Reviews.Application.Queries.GetReviewsByUserId;

public sealed record class GetReviewsByUserIdQuery(Guid UserId, int Page = 1, int PageSize = 10) : IRequest<Result<GetReviewsByUserIdResponse>>;
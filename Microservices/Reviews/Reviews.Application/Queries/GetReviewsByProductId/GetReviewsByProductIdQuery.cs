using MediatR;
using Shared.Domain;

namespace Reviews.Application.Queries.GetReviews;

public sealed record GetReviewsByProductIdQuery(int ProductId, int Page = 1, int PageSize = 10) : IRequest<Result<GetReviewsByProductIdResponse>>;

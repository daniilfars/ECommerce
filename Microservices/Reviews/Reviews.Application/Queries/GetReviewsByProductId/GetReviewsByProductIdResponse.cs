using Reviews.Application.DTOs;

namespace Reviews.Application.Queries.GetReviews;

public sealed record GetReviewsByProductIdResponse(List<GetReviewsDto> Reviews, int TotalCount, int Page, int PageSize);
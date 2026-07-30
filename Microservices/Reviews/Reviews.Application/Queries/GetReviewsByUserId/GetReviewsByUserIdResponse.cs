using Reviews.Application.DTOs;

namespace Reviews.Application.Queries.GetReviewsByUserId;

public sealed record GetReviewsByUserIdResponse(List<GetReviewsDto> Reviews, int TotalCount, int Page, int PageSize);
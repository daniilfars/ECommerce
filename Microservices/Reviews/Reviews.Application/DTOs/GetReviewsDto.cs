namespace Reviews.Application.DTOs;

public sealed record GetReviewsDto(Guid UserId, int Id, int ProductId, string Text, int Stars);
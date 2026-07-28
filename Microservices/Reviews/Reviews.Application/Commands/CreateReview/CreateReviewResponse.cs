namespace Reviews.Application.Commands.CreateReview;

public sealed record CreateReviewResponse(Guid UserId, int Id, int ProductId, string Text, int Stars);
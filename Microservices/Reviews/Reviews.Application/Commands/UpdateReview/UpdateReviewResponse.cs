namespace Reviews.Application.Commands.UpdateReview;

public sealed record UpdateReviewResponse(Guid UserId, int Id, int ProductId, string Text, int Stars);

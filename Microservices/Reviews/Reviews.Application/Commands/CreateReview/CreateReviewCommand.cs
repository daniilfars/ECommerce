using MediatR;
using Shared.Domain;

namespace Reviews.Application.Commands.CreateReview;

public sealed record CreateReviewCommand(Guid UserId, int ProductId, string Text, int Stars) : IRequest<Result<CreateReviewResponse>>;
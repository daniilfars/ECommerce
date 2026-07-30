using MediatR;
using Shared.Domain;

namespace Reviews.Application.Commands.UpdateReview;

public sealed record UpdateReviewCommand(Guid UserId, int Id, string? Text = null, int? Stars = null) : IRequest<Result<UpdateReviewResponse>>;
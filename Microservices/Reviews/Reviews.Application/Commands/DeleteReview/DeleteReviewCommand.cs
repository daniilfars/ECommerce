using MediatR;
using Shared.Domain;

namespace Reviews.Application.Commands.DeleteReview;

public sealed record DeleteReviewCommand(Guid UserId, int Id) : IRequest<Result>;
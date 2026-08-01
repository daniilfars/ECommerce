using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.Interfaces;
using Shared.Contracts;
using Shared.Domain;

namespace Reviews.Application.Commands.DeleteReview;

public sealed class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IReviewsDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteReviewHandler(IReviewsDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (review is null)
            return Result.Failure("Отзыв не найден");

        if (review.UserId != request.UserId)
            return Result.Failure("Нет доступа к отзыву");

        _context.Reviews.Remove(review);

        await _publishEndpoint.Publish<ReviewDeleted>(new { ProductId = review.ProductId, Stars = review.Stars }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
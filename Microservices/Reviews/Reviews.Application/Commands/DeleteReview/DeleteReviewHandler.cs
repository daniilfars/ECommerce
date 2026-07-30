using MediatR;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.Interfaces;
using Shared.Domain;

namespace Reviews.Application.Commands.DeleteReview;

public sealed class DeleteReviewHandler : IRequestHandler<DeleteReviewCommand, Result>
{
    private readonly IReviewsDbContext _context;

    public DeleteReviewHandler(IReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (review is null)
            return Result.Failure("Отзыв не найден");

        if (review.UserId != request.UserId)
            return Result.Failure("Нет доступа к отзыву");

        _context.Reviews.Remove(review);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
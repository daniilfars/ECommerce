using MediatR;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.Interfaces;
using Shared.Domain;

namespace Reviews.Application.Commands.UpdateReview;

public sealed class UpdateReviewHandler : IRequestHandler<UpdateReviewCommand, Result<UpdateReviewResponse>>
{
    private readonly IReviewsDbContext _context;

    public UpdateReviewHandler(IReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<UpdateReviewResponse>> Handle(UpdateReviewCommand request, CancellationToken cancellationToken)
    {
        var review = await _context.Reviews.FirstOrDefaultAsync(r => r.Id == request.Id, cancellationToken);
        if (review is null)
            return Result<UpdateReviewResponse>.Failure("Отзыв не найден");

        if (review.UserId != request.UserId)
            return Result<UpdateReviewResponse>.Failure("Нет доступа к отзыву");

        if (request.Text != null)
            review.UpdateText(request.Text);

        if (request.Stars != null)
            review.UpdateStars((int)request.Stars);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<UpdateReviewResponse>.Success(new UpdateReviewResponse(review.UserId, review.Id, review.ProductId, review.Text, review.Stars));
    }
}

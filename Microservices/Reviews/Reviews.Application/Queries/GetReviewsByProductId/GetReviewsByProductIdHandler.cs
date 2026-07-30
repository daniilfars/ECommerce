using MediatR;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.Interfaces;
using Shared.Domain;
using Reviews.Application.DTOs;

namespace Reviews.Application.Queries.GetReviews;

public sealed class GetReviewsByProductIdHandler : IRequestHandler<GetReviewsByProductIdQuery, Result<GetReviewsByProductIdResponse>>
{
    private readonly IReviewsDbContext _context;

    public GetReviewsByProductIdHandler(IReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetReviewsByProductIdResponse>> Handle(GetReviewsByProductIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reviews.Where(r => r.ProductId == request.ProductId).AsNoTracking();

        var totalCount = query.Count();

        var reviews = await query
            .OrderBy(r => r.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new GetReviewsDto(r.UserId, r.Id, r.ProductId, r.Text, r.Stars))
            .ToListAsync(cancellationToken);

        return Result<GetReviewsByProductIdResponse>.Success(new GetReviewsByProductIdResponse(reviews, totalCount, request.Page, request.PageSize));
    }
}
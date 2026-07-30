using MediatR;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.DTOs;
using Reviews.Application.Interfaces;
using Shared.Domain;

namespace Reviews.Application.Queries.GetReviewsByUserId;

public sealed class GetReviewsByUserIdHandler : IRequestHandler<GetReviewsByUserIdQuery, Result<GetReviewsByUserIdResponse>>
{
    private readonly IReviewsDbContext _context;

    public GetReviewsByUserIdHandler(IReviewsDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetReviewsByUserIdResponse>> Handle(GetReviewsByUserIdQuery request, CancellationToken cancellationToken)
    {
        var query = _context.Reviews.Where(r => r.UserId == request.UserId).AsNoTracking();

        var totalCount = query.Count();

        var reviews = await query
            .OrderByDescending(r => r.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(r => new GetReviewsDto(r.UserId, r.Id, r.ProductId, r.Text, r.Stars))
            .ToListAsync(cancellationToken);

        return Result<GetReviewsByUserIdResponse>.Success(new GetReviewsByUserIdResponse(reviews, totalCount, request.Page, request.PageSize));
    }
}

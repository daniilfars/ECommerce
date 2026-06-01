using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Ordering.Application.Interfaces;
using Shared.Domain;

namespace Modules.Ordering.Application.Queries.GetOrders;

public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, Result<GetOrdersResponse>>
{
    private readonly IOrderingDbContext _context;

    public GetOrdersHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetOrdersResponse>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _context.Orders.Where(o => o.UserId == request.UserId).CountAsync(cancellationToken);

        var orders = await _context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == request.UserId)
            .OrderByDescending(o => o.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(o => new OrderDto(o.Id, o.UserId, o.ShippingAddress, o.Status.ToString(), o.TotalAmount, o.Items.Count()))
            .ToListAsync(cancellationToken);

        return Result<GetOrdersResponse>.Success(new GetOrdersResponse(orders, totalCount, request.Page, request.PageSize));
    }
}

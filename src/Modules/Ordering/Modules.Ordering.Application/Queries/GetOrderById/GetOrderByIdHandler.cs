using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Ordering.Application.Interfaces;
using Shared.Domain;

namespace Modules.Ordering.Application.Queries.GetOrderById;

public class GetOrderByIdHandler : IRequestHandler<GetOrderByIdQuery, Result<GetOrderByIdResponse>>
{
    private readonly IOrderingDbContext _context;

    public GetOrderByIdHandler(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetOrderByIdResponse>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.Include(o => o.Items).AsNoTracking().FirstOrDefaultAsync(o => o.Id == request.OrderId);
        if (order is null)
            return Result<GetOrderByIdResponse>.Failure("Заказ не найден");

        if(order.UserId != request.UserId)
            return Result<GetOrderByIdResponse>.Failure("Нет доступа к заказу");

        var items = new List<OrderItemDto>(order.Items.Select(i => new OrderItemDto(i.Id, i.ProductId, i.ProductName, i.PriceAmount, i.PriceCurrency, i.Quantity, i.TotalPrice, i.ImageUrl)));

        return Result<GetOrderByIdResponse>.Success(new GetOrderByIdResponse(order.UserId, order.Status.ToString(), order.ShippingAddress, order.TotalAmount, items));
    }
}

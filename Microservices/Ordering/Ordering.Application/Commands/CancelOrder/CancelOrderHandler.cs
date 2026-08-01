using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Shared.Contracts;
using Shared.Domain;

namespace Ordering.Application.Commands.CancelOrder;

public class CancelOrderHandler : IRequestHandler<CancelOrderCommand, Result>
{
    private readonly IOrderingDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public CancelOrderHandler(IOrderingDbContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _context.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken);
        if (order is null)
            return Result.Failure("Заказ не найден");

        if(!request.IsAdmin && order.UserId != request.UserId)
            return Result.Failure("Нет доступа к заказу");

        var result = order.Cancel();
        if (result.IsFailure)
            return Result.Failure(result.Error!);

        await _publishEndpoint.Publish<OrderCancelled>(new { Items = order.Items.Select(i => new { i.ProductId, i.Quantity }) }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
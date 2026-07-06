using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Ordering.Domain;
using Shared.Contracts;

namespace Ordering.Application.Consumers;

public class StockReserveFailedConsumer : IConsumer<StockReserveFailed>
{
    private readonly IOrderingDbContext _context;

    public StockReserveFailedConsumer(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockReserveFailed> context)
    {
        var message = context.Message;

        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == message.OrderId, context.CancellationToken);

        if (order is null)
            return;

        if (order.Status == OrderStatus.Cancelled)
            return;

        var result = order.Cancel();
        if (result.IsFailure)
        {

            throw new InvalidOperationException($"Ошибка отмены заказа {order.Id}: {result.Error}");
        }

        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Shared.Contracts;

namespace Ordering.Application.Consumers;

public class StockReservedConsumer : IConsumer<StockReserved>
{
    private readonly IOrderingDbContext _context;

    public StockReservedConsumer(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockReserved> context)
    {
        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == context.Message.OrderId, context.CancellationToken);
        if (order is null)
            return;

        order.Confirm();

        await _context.SaveChangesAsync(context.CancellationToken);
    }
}
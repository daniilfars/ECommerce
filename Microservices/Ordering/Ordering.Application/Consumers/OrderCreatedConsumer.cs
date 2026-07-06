using MassTransit;
using Ordering.Application.Interfaces;
using Ordering.Domain;
using Shared.Contracts;

namespace Ordering.Application.Consumers;

public class OrderCreatedConsumer : IConsumer<OrderCreated>
{
    private readonly IOrderingDbContext _context;

    public OrderCreatedConsumer(IOrderingDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<OrderCreated> context)
    {
        var message = context.Message;

        var orderResult = Order.Create(message.OrderId, message.UserId, message.ShippingAddress);
        if (orderResult.IsFailure)
            throw new InvalidOperationException(orderResult.Error);

        var order = orderResult.Value;

        foreach (var itemDto in message.Items)
        {
            var itemResult = OrderItem.Create(itemDto.ProductId, itemDto.ProductName, itemDto.Price, itemDto.Quantity, itemDto.ImageUrl);
            if (itemResult.IsFailure)
                throw new InvalidOperationException(itemResult.Error);

            order.AddItem(itemResult.Value!);
        }

        await using var transaction = await _context.Database.BeginTransactionAsync(context.CancellationToken);

        _context.Orders.Add(order);

        await context.Publish<StockReserveRequested>(new
        {
            OrderId = order.Id,
            Items = message.Items.Select(i => new { i.ProductId, i.Quantity }).ToArray()
        }, context.CancellationToken);

        await _context.SaveChangesAsync(context.CancellationToken);
        await transaction.CommitAsync(context.CancellationToken);
    }
}
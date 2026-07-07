using Catalog.Application.Interfaces;
using EFCore.PostgresExtensions.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class StockReserveRequestedConsumer : IConsumer<StockReserveRequested>
{
    private readonly ICatalogDbContext _context;

    public StockReserveRequestedConsumer(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<StockReserveRequested> context)
    {
        var message = context.Message;

        var groupedItems = message.Items.GroupBy(i => i.ProductId).Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) }).ToList();

        var ids = groupedItems.Select(p => p.ProductId).ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync(context.CancellationToken);

        var products = await _context.Products.Where(p => ids.Contains(p.Id)).ForUpdate().ToListAsync(context.CancellationToken);
        // Вначале валидацию делаем
        foreach (var item in groupedItems)
        {
            var product = products.FirstOrDefault(p => p.Id == item.ProductId);

            if (product is null || product.StockQuantity < item.Quantity)
            {
                var reason = product is null
                    ? $"Товар с ID {item.ProductId} не найден"
                    : $"Недостаточно товара с ID {item.ProductId} на складе";

                await context.Publish<StockReserveFailed>(new
                {
                    OrderId = message.OrderId,
                    Reason = reason
                }, context.CancellationToken);

                await transaction.RollbackAsync(context.CancellationToken);
                return;
            }
        }

        // Затем уже изменяем
        foreach (var item in groupedItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.ReserveStock(item.Quantity);
        }

        await context.Publish<StockReserved>(new {message.OrderId});

        await _context.SaveChangesAsync(context.CancellationToken);
        await transaction.CommitAsync(context.CancellationToken);
    }
}
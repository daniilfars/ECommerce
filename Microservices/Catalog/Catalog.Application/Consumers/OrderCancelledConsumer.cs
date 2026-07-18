using Catalog.Application.Interfaces;
using EFCore.PostgresExtensions.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class OrderCancelledConsumer : IConsumer<OrderCancelled>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;

    public OrderCancelledConsumer(ICatalogDbContext context, ICatalogCacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<OrderCancelled> context)
    {
        var message = context.Message;

        var groupedItems = message.Items.GroupBy(i => i.ProductId)
            .Select(g => new {ProductId = g.Key, Quantity = g.Sum(x => x.Quantity)})
            .ToList();

        var ids = groupedItems.Select(g => g.ProductId).ToList();

        var products = await _context.Products.Where(p => ids.Contains(p.Id)).ForUpdate().ToListAsync(context.CancellationToken);

        foreach(var product in products)
        {
            var item = groupedItems.First(i => i.ProductId == product.Id);
            product.ReturnStock(item.Quantity);
        }

        await _context.SaveChangesAsync(context.CancellationToken);

        var cacheTasks = products.Select(p => _cacheService.ClearProductByIdAsync(p.Id));
        await Task.WhenAll(cacheTasks);

        await _cacheService.ClearCatalogPagesAsync();
    }
}
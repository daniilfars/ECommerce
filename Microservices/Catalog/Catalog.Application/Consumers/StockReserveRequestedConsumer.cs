using Catalog.Application.Interfaces;
using EFCore.PostgresExtensions.Extensions;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class StockReserveRequestedConsumer : IConsumer<StockReserveRequested>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;

    public StockReserveRequestedConsumer(ICatalogDbContext context, ICatalogCacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task Consume(ConsumeContext<StockReserveRequested> context)
    {
        var message = context.Message;

        var groupedItems = message.Items
            .GroupBy(i => i.ProductId)
            .Select(g => new { ProductId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .OrderBy(x => x.ProductId)
            .ToList();

        var ids = groupedItems.Select(p => p.ProductId).ToList();

        var products = await _context.Products
            .Where(p => ids.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ForUpdate()
            .ToListAsync(context.CancellationToken);

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

                await _context.SaveChangesAsync(context.CancellationToken);

                return;
            }
        }

        // Затем уже изменяем
        foreach (var item in groupedItems)
        {
            var product = products.First(p => p.Id == item.ProductId);
            product.ReserveStock(item.Quantity);
        }

        await context.Publish<StockReserved>(new {message.OrderId}, context.CancellationToken);
        await context.Publish<ProductsStockChanged>(new {
            Products = products.Select(p => new {
                ProductId = p.Id, 
                StockQuantity = p.StockQuantity}
            ).ToArray()
        }, context.CancellationToken);

        await _context.SaveChangesAsync(context.CancellationToken);

        var cacheTasks = products.Select(p => _cacheService.ClearProductByIdAsync(p.Id));
        await Task.WhenAll(cacheTasks);

        await _cacheService.ClearCatalogPagesAsync();
    }
}
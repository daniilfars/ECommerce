using Catalog.Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class ReviewCreatedConsumer : IConsumer<ReviewCreated>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cache;

    public ReviewCreatedConsumer(ICatalogDbContext context, ICatalogCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Consume(ConsumeContext<ReviewCreated> context)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == context.Message.ProductId, context.CancellationToken);
        if (product is null)
            return;

        product.ReviewCreated(context.Message.Stars);

        await _context.SaveChangesAsync(context.CancellationToken);

        await _cache.ClearProductByIdAsync(product.Id);
        await _cache.ClearCatalogPagesAsync();
    }
}
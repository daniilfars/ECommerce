using Catalog.Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class ReviewDeletedConsumer : IConsumer<ReviewDeleted>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cache;

    public ReviewDeletedConsumer(ICatalogDbContext context, ICatalogCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Consume(ConsumeContext<ReviewDeleted> context)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == context.Message.ProductId, context.CancellationToken);
        if (product is null)
            return;

        product.ReviewDeleted(context.Message.Stars);

        await _context.SaveChangesAsync(context.CancellationToken);

        await _cache.ClearProductByIdAsync(product.Id);
        await _cache.ClearCatalogPagesAsync();
    }
}

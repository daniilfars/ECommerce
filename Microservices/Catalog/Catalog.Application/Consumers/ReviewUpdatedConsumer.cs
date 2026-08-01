using Catalog.Application.Interfaces;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts;

namespace Catalog.Application.Consumers;

public class ReviewUpdatedConsumer : IConsumer<ReviewUpdated>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cache;

    public ReviewUpdatedConsumer(ICatalogDbContext context, ICatalogCacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task Consume(ConsumeContext<ReviewUpdated> context)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == context.Message.ProductId, context.CancellationToken);
        if (product is null)
            return;

        product.ReviewUpdated(context.Message.DifferenceStars);

        await _context.SaveChangesAsync(context.CancellationToken);

        await _cache.ClearProductByIdAsync(product.Id);
        await _cache.ClearCatalogPagesAsync();
    }
}

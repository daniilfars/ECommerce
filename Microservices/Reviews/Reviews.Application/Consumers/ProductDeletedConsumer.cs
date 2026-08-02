using MassTransit;
using Microsoft.EntityFrameworkCore;
using Reviews.Application.Interfaces;
using Shared.Contracts;

namespace Reviews.Application.Consumers;

public class ProductDeletedConsumer : IConsumer<ProductDeleted>
{
    private readonly IReviewsDbContext _context;

    public ProductDeletedConsumer(IReviewsDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<ProductDeleted> context)
    {
        await _context.Reviews.Where(r => r.ProductId == context.Message.ProductId).ExecuteDeleteAsync(context.CancellationToken);
    }
}

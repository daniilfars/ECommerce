using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Shared.Domain;
using StackExchange.Redis;
using System.Text.Json;

namespace Catalog.Application.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IDatabase _cache;

    public GetProductByIdHandler(ICatalogDbContext context, IDatabase cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var key = $"catalog:product:{request.Id}";

        var cached = await _cache.StringGetAsync(key);
        if(cached.HasValue)
        {
            ReadOnlyMemory<byte> memory = cached;

            var cachedResponse = JsonSerializer.Deserialize<GetProductByIdResponse>(memory.Span);
            if (cachedResponse != null)
                return Result<GetProductByIdResponse>.Success(cachedResponse);
        }

        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
            return Result<GetProductByIdResponse>.Failure("Товар не найден");

        var response = new GetProductByIdResponse(product.Id, product.Name, product.Price, product.StockQuantity, product.ImageUrl, product.ReviewCount > 0 ? (decimal)product.TotalStars / product.ReviewCount : 0, product.ReviewCount);

        await _cache.StringSetAsync(key, JsonSerializer.SerializeToUtf8Bytes(response), TimeSpan.FromMinutes(60));

        return Result<GetProductByIdResponse>.Success(response);
    }
}
using Catalog.Application.Interfaces;
using MassTransit;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Domain;
using StackExchange.Redis;
using System.Text.Json;

namespace Catalog.Application.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<GetProductsResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly IDatabase _cache;

    public GetProductsHandler(ICatalogDbContext context, IDatabase cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<GetProductsResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        if (!request.HasFilter)
        {
            var cached = await _cache.StringGetAsync($"catalog:page:{request.Page}:size:{request.PageSize}");
            if (cached.HasValue)
            {
                ReadOnlyMemory<byte> memory = cached;

                var cachedResponse = JsonSerializer.Deserialize<GetProductsResponse>(memory.Span);
                if (cachedResponse != null)
                    return Result<GetProductsResponse>.Success(cachedResponse);
            }
        }

        var query = _context.Products.AsNoTracking();

        if (request.MinPrice != null) query = query.Where(p => p.Price >= request.MinPrice);
        if (request.MaxPrice != null) query = query.Where(p => p.Price <= request.MaxPrice);

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .OrderBy(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.StockQuantity, p.ImageUrl))
            .ToListAsync(cancellationToken);

        var response = new GetProductsResponse(products, totalCount, request.Page, request.PageSize);

        if (!request.HasFilter)
            await _cache.StringSetAsync($"catalog:page:{request.Page}:size:{request.PageSize}", JsonSerializer.SerializeToUtf8Bytes(response), TimeSpan.FromMinutes(60));

        return Result<GetProductsResponse>.Success(response);
    }
}
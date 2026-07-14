using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
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
        var key = $"catalog:page:{request.Page}:size:{request.PageSize}";

        var cached = await _cache.StringGetAsync(key);
        if(cached.HasValue)
        {
            var cachedResponse = JsonSerializer.Deserialize<GetProductsResponse>(cached.ToString());
            if(cachedResponse != null)
                return Result<GetProductsResponse>.Success(cachedResponse);
        }

        var totalCount = await _context.Products.CountAsync(cancellationToken);

        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.StockQuantity, p.ImageUrl))
            .ToListAsync(cancellationToken);

        var response = new GetProductsResponse(products, totalCount, request.Page, request.PageSize);

        await _cache.StringSetAsync(key, JsonSerializer.Serialize(response), TimeSpan.FromMinutes(60));

        return Result<GetProductsResponse>.Success(response);
    }
}
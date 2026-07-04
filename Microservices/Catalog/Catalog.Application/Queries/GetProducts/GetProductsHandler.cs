using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Shared.Domain;

namespace Catalog.Application.Queries.GetProducts;

public class GetProductsHandler : IRequestHandler<GetProductsQuery, Result<GetProductsResponse>>
{
    private readonly ICatalogDbContext _context;

    public GetProductsHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetProductsResponse>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var totalCount = await _context.Products.CountAsync(cancellationToken);

        var products = await _context.Products
            .AsNoTracking()
            .OrderBy(p => p.Id)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.StockQuantity, p.ImageUrl))
            .ToListAsync(cancellationToken);

        return Result<GetProductsResponse>.Success(new GetProductsResponse(products, totalCount, request.Page, request.PageSize));
    }
}
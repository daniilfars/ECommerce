using MediatR;
using Catalog.Application.Interfaces;
using Catalog.Domain;
using Shared.Domain;

namespace Catalog.Application.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;

    public CreateProductHandler(ICatalogDbContext context, ICatalogCacheService cacheService)
    {
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(request.Name, request.Price, request.StockQuantity);
        if(productResult.IsFailure)
            return Result<CreateProductResponse>.Failure(productResult.Error!);

        var product = productResult.Value!;

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.ClearCatalogPagesAsync();

        return Result<CreateProductResponse>.Success(new CreateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity, product.ImageUrl));
    }
}
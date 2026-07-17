using MediatR;
using Catalog.Application.Interfaces;
using Catalog.Domain;
using Shared.Domain;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

    public CreateProductHandler(ICatalogDbContext context, ICatalogCacheService cacheService, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var productResult = Product.Create(request.Name, request.Price, request.StockQuantity);
        if(productResult.IsFailure)
            return Result<CreateProductResponse>.Failure(productResult.Error!);

        var product = productResult.Value!;

        _context.Products.Add(product);

        await _publishEndpoint.Publish<ProductCreated>(new
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.ClearCatalogPagesAsync();

        return Result<CreateProductResponse>.Success(new CreateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity, product.ImageUrl));
    }
}
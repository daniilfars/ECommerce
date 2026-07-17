using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Shared.Domain;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

    public UpdateProductHandler(ICatalogDbContext context, ICatalogCacheService cacheService, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<UpdateProductResponse>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
            return Result<UpdateProductResponse>.Failure("Товар не найден");

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            var updateResult = product.UpdateName(request.Name);
            if(updateResult.IsFailure)
                return Result<UpdateProductResponse>.Failure(updateResult.Error!);
        }

        if (request.Price.HasValue)
        {
            var updateResult = product.UpdatePrice(request.Price.Value);
            if(updateResult.IsFailure)
                return Result<UpdateProductResponse>.Failure(updateResult.Error!);
        }

        if (request.StockQuantity.HasValue)
        {
            var updateResult = product.UpdateStockQuantity(request.StockQuantity.Value);
            if (updateResult.IsFailure)
                return Result<UpdateProductResponse>.Failure(updateResult.Error!);
        }

        await _publishEndpoint.Publish<ProductUpdated>(new
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            ImageUrl = product.ImageUrl,
            StockQuantity = product.StockQuantity
        }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        await _cacheService.ClearProductByIdAsync(request.Id);
        await _cacheService.ClearCatalogPagesAsync();

        return Result<UpdateProductResponse>.Success(new UpdateProductResponse(product.Id, product.Name, product.Price, product.StockQuantity, product.ImageUrl));
    }
}
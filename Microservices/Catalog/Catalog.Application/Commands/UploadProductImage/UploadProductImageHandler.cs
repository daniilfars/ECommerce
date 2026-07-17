using MediatR;
using Catalog.Application.Interfaces;
using Shared.Domain;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Commands.UploadProductImage;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, Result<UploadProductImageResponse>>
{
    private readonly IImageStorageService _service;
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;
    private readonly IPublishEndpoint _publishEndpoint;

    public UploadProductImageHandler(IImageStorageService service, ICatalogDbContext context, ICatalogCacheService cacheService, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _context = context;
        _cacheService = cacheService;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result<UploadProductImageResponse>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FindAsync([request.ProductId], cancellationToken);
        if (product is null)
            return Result<UploadProductImageResponse>.Failure("Товар не найден");

        var extension = request.ContentType switch
        {
            "image/jpeg" => ".jpg",
            "image/png" => ".png",
            "image/webp" => ".webp",
            _ => null
        };

        if (extension is null)
            return Result<UploadProductImageResponse>.Failure("Неподдерживаемый формат изображения");

        string? oldImageUrl = product.ImageUrl;
        var objectName = $"products/{request.ProductId}/{Guid.NewGuid()}{extension}";

        var url = await _service.UploadAsync(objectName, request.FileStream, request.ContentType, cancellationToken);

        try
        {
            product.SetImageUrl(url);

            await _publishEndpoint.Publish<ProductUpdated>(new
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                ImageUrl = product.ImageUrl,
                StockQuantity = product.StockQuantity
            }, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
        }
        catch
        {
            try
            {
                await _service.DeleteAsync(objectName, cancellationToken);
            }
            catch
            {
            }
            throw;
        }

        if (!string.IsNullOrEmpty(oldImageUrl))
        {
            try
            {
                var deleteObjectName = _service.GetObjectNameFromUrl(oldImageUrl);
                await _service.DeleteAsync(deleteObjectName, cancellationToken);
            }
            catch
            {
            }
        }

        await _cacheService.ClearProductByIdAsync(request.ProductId);
        await _cacheService.ClearCatalogPagesAsync();

        return Result<UploadProductImageResponse>.Success(new UploadProductImageResponse(url));
    }
}
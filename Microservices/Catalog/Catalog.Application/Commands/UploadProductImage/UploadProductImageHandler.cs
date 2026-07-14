using MediatR;
using Catalog.Application.Interfaces;
using Shared.Domain;

namespace Catalog.Application.Commands.UploadProductImage;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, Result<UploadProductImageResponse>>
{
    private readonly IImageStorageService _service;
    private readonly ICatalogDbContext _context;
    private readonly ICatalogCacheService _cacheService;

    public UploadProductImageHandler(IImageStorageService service, ICatalogDbContext context, ICatalogCacheService cacheService)
    {
        _service = service;
        _context = context;
        _cacheService = cacheService;
    }

    public async Task<Result<UploadProductImageResponse>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
        await using var fileStream = request.FileStream;

        var product = await _context.Products.FindAsync(request.ProductId, cancellationToken);
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

        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        string? oldImageUrl = product.ImageUrl;
        var objectName = $"products/{request.ProductId}/{Guid.NewGuid()}{extension}";

        var url = await _service.UploadAsync(objectName, fileStream, request.ContentType, cancellationToken);
        product.SetImageUrl(url);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        if (oldImageUrl != null)
        {
            var deleteObjectName = _service.GetObjectNameFromUrl(oldImageUrl);
            await _service.DeleteAsync(deleteObjectName, cancellationToken);
        }

        await _cacheService.ClearProductByIdAsync(request.ProductId);
        await _cacheService.ClearCatalogPagesAsync();

        return Result<UploadProductImageResponse>.Success(new UploadProductImageResponse(url));
    }
}

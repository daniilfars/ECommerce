using MediatR;
using Modules.Catalog.Application.Interfaces;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.UploadProductImage;

public class UploadProductImageHandler : IRequestHandler<UploadProductImageCommand, Result<UploadProductImageResponse>>
{
    private readonly IImageStorageService _service;
    private readonly ICatalogDbContext _context;

    public UploadProductImageHandler(IImageStorageService service, ICatalogDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task<Result<UploadProductImageResponse>> Handle(UploadProductImageCommand request, CancellationToken cancellationToken)
    {
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

        var objectName = $"products/{request.ProductId}/{Guid.NewGuid()}{extension}";

        var url = await _service.UploadAsync(objectName, request.FileStream, request.ContentType, cancellationToken);
        product.SetImageUrl(url);

        await _context.SaveChangesAsync(cancellationToken);

        return Result<UploadProductImageResponse>.Success(new UploadProductImageResponse(url));
    }
}
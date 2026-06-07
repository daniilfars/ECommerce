using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Application.Interfaces;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IImageStorageService _service;
    private readonly ICatalogDbContext _context;

    public DeleteProductHandler(IImageStorageService service, ICatalogDbContext context)
    {
        _service = service;
        _context = context;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
            return Result.Failure("Товар не найден");

        if (product.ImageUrl != null)
        {
            var objectName = _service.GetObjectNameFromUrl(product.ImageUrl);
            await _service.DeleteAsync(objectName, cancellationToken);
        }

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
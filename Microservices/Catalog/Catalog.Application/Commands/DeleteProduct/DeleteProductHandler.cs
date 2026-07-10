using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Shared.Domain;
using MassTransit;
using Shared.Contracts;

namespace Catalog.Application.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly IImageStorageService _service;
    private readonly ICatalogDbContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public DeleteProductHandler(IImageStorageService service, ICatalogDbContext context, IPublishEndpoint publishEndpoint)
    {
        _service = service;
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

        var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);

        if (product is null)
            return Result.Failure("Товар не найден");

        if (product.ImageUrl != null)
        {
            var objectName = _service.GetObjectNameFromUrl(product.ImageUrl);
            await _service.DeleteAsync(objectName, cancellationToken);
        }

        _context.Products.Remove(product);

        await _publishEndpoint.Publish<ProductDeleted>(new { ProductId = product.Id }, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        return Result.Success();
    }
}
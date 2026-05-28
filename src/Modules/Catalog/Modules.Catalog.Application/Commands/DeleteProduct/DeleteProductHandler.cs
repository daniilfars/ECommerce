using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Application.Interfaces;
using Modules.Catalog.Domain;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.DeleteProduct;

public class DeleteProductHandler : IRequestHandler<DeleteProductCommand, Result>
{
    private readonly ICatalogDbContext _context;

    public DeleteProductHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        //var productId = ProductId.From(request.Id);
        var product = await _context.Products.FirstOrDefaultAsync(p => p._id == request.Id, cancellationToken);

        if (product is null)
            return Result.Failure("Товар не найден");

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
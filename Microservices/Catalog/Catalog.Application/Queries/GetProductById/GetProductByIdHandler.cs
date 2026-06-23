using MediatR;
using Microsoft.EntityFrameworkCore;
using Catalog.Application.Interfaces;
using Catalog.Domain;
using Shared.Domain;

namespace Catalog.Application.Queries.GetProductById;

public class GetProductByIdHandler : IRequestHandler<GetProductByIdQuery, Result<GetProductByIdResponse>>
{
    private readonly ICatalogDbContext _context;

    public GetProductByIdHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<GetProductByIdResponse>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products.AsNoTracking().FirstOrDefaultAsync(p => p.Id == request.Id, cancellationToken);
        if (product is null)
            return Result<GetProductByIdResponse>.Failure("Товар не найден");

        return Result<GetProductByIdResponse>.Success(new GetProductByIdResponse(product.Id, product.Name, product.Price.Amount, product.Price.Currency, product.ImageUrl));
    }
}
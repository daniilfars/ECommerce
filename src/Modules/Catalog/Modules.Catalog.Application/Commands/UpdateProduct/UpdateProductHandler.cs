using MediatR;
using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Application.Interfaces;
using Modules.Catalog.Domain;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.UpdateProduct;

public class UpdateProductHandler : IRequestHandler<UpdateProductCommand, Result<UpdateProductResponse>>
{
    private readonly ICatalogDbContext _context;

    public UpdateProductHandler(ICatalogDbContext context)
    {
        _context = context;
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

        if (request.PriceAmount.HasValue && !string.IsNullOrWhiteSpace(request.PriceCurrency))
        {
            var price = Money.Create(request.PriceAmount.Value, request.PriceCurrency);
            if (price.IsFailure)
                return Result<UpdateProductResponse>.Failure(price.Error!);
            product.UpdatePrice(price.Value!);
        }
        else if (request.PriceAmount.HasValue || !string.IsNullOrWhiteSpace(request.PriceCurrency))
        {
            return Result<UpdateProductResponse>.Failure("Для обновления цены нужны оба поля: PriceAmount и PriceCurrency");
        }

        await _context.SaveChangesAsync(cancellationToken);

        return Result<UpdateProductResponse>.Success(new UpdateProductResponse(product.Id, product.Name, product.PriceAmount, product.PriceCurrency));
    }
}
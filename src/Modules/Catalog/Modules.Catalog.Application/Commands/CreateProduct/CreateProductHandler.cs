using MediatR;
using Modules.Catalog.Application.Interfaces;
using Modules.Catalog.Domain;
using Shared.Domain;

namespace Modules.Catalog.Application.Commands.CreateProduct;

public sealed class CreateProductHandler : IRequestHandler<CreateProductCommand, Result<CreateProductResponse>>
{
    private readonly ICatalogDbContext _context;

    public CreateProductHandler(ICatalogDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CreateProductResponse>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var moneyResult = Money.Create(request.PriceAmount, request.PriceCurrency);
        if (moneyResult.IsFailure)
            return Result<CreateProductResponse>.Failure(moneyResult.Error!);

        var productResult = Product.Create(request.Name, moneyResult.Value!);
        if(productResult.IsFailure)
            return Result<CreateProductResponse>.Failure(productResult.Error!);

        var product = productResult.Value!;

        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<CreateProductResponse>.Success(new CreateProductResponse(product.Id, product.Name, product.PriceAmount, product.PriceCurrency, product.ImageUrl));
    }
}

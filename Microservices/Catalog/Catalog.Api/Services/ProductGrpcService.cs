using Catalog.Application.Queries.GetProductById;
using CatalogGrpc;
using Grpc.Core;
using MediatR;

namespace Catalog.Api.Services;

public class ProductGrpcService : ProductService.ProductServiceBase
{
    private readonly IMediator _mediator;
    
    public ProductGrpcService(IMediator mediator)
    {
        _mediator = mediator;
    }

    public override async Task<ProductReply> GetProduct(GetProductRequest request, ServerCallContext context)
    {
        var result = await _mediator.Send(new GetProductByIdQuery(request.Id), context.CancellationToken);

        if (result.IsFailure)
            throw new RpcException(new Status(StatusCode.NotFound, result.Error!));

        var product = result.Value;

        return new ProductReply
        {
            Id = product!.Id,
            Name = product.Name,
            PriceInCents = (long)(product.Price * 100),
            StockQuantity = product.StockQuantity,
            ImageUrl = product.ImageUrl
        };
    }
}
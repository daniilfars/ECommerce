using MediatR;
using Basket.Application.Queries.GetBasket;
using Basket.Domain;
using Shared.Domain;
using StackExchange.Redis;
using CatalogGrpc;
using Grpc.Core;

namespace Basket.Application.Commands.AddItemToBasket;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IDatabase _redisDb;
    private readonly ProductService.ProductServiceClient _productServiceClient;

    public AddItemToBasketHandler(IBasketRepository basketRepository, IDatabase redisDb, ProductService.ProductServiceClient productServiceClient)
    {
        _basketRepository = basketRepository;
        _redisDb = redisDb;
        _productServiceClient = productServiceClient;
    }

    public async Task<Result<GetBasketResponse>> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var product = await _productServiceClient.GetProductAsync(new GetProductRequest { Id = request.ProductId }, cancellationToken: cancellationToken);

            var key = $"product:{product.Id}:stock";

            await _redisDb.StringSetAsync(key, product.StockQuantity, TimeSpan.FromDays(2));

            if (product.StockQuantity < request.Quantity)
                return Result<GetBasketResponse>.Failure("Недостаточно товара на складе");

            var itemResult = BasketItem.Create(product.Id, product.Name, product.PriceInCents / 100m, request.Quantity, product.ImageUrl);
            if (itemResult.IsFailure)
                return Result<GetBasketResponse>.Failure(itemResult.Error!);

            var basket = await _basketRepository.GetBasketAsync(request.UserId);
            if (basket == null)
                basket = Domain.Basket.Create(request.UserId);

            var item = itemResult.Value!;

            basket.AddItem(item);

            await _basketRepository.SaveBasketAsync(basket);

            return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity, i.TotalPrice, i.ImageUrl)).ToList(), basket.TotalAmount));
        }
        catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
        {
            return Result<GetBasketResponse>.Failure(ex.Message);
        }
        catch (RpcException ex)
        {
            return Result<GetBasketResponse>.Failure($"Ошибка связи с каталогом: {ex.Status.Detail}");
        }
    }
}
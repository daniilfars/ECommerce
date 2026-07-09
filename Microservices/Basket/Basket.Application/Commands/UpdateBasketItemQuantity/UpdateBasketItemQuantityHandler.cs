using Basket.Application.Models;
using Basket.Application.Queries.GetBasket;
using Basket.Domain;
using MediatR;
using Shared.Domain;
using StackExchange.Redis;
using System.Net.Http.Json;

namespace Basket.Application.Commands.UpdateBasketItemQuantity;

public class UpdateBasketItemQuantityHandler : IRequestHandler<UpdateBasketItemQuantityCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabase _redisDb;

    public UpdateBasketItemQuantityHandler(IBasketRepository basketRepository, IHttpClientFactory httpClientFactory, IDatabase redisDb)
    {
        _basketRepository = basketRepository;
        _httpClientFactory = httpClientFactory;
        _redisDb = redisDb;
    }

    public async Task<Result<GetBasketResponse>> Handle(UpdateBasketItemQuantityCommand request, CancellationToken cancellationToken)
    {
        int finalStockQuantity;
        var key = $"product:{request.ProductId}:stock";
        var stockQuantity = await _redisDb.StringGetAsync(key);

        if (!stockQuantity.HasValue)
        {
            var client = _httpClientFactory.CreateClient();
            var response = await client.GetAsync($"http://catalog-api:8080/api/Catalog/{request.ProductId}", cancellationToken);

            if (!response.IsSuccessStatusCode)
                return Result<GetBasketResponse>.Failure("Товар не найден");

            var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);

            if (product is null)
                return Result<GetBasketResponse>.Failure("Товар не найден");

            finalStockQuantity = product.StockQuantity;
            _ = _redisDb.StringSetAsync(key, finalStockQuantity, TimeSpan.FromDays(2));
        }
        else
        {
            finalStockQuantity = (int)stockQuantity;
        }

        if (finalStockQuantity < request.Quantity)
            return Result<GetBasketResponse>.Failure("Недостаточно товара на складе");

        var basket = await _basketRepository.GetBasketAsync(request.UserId);
        if (basket == null)
            basket = Domain.Basket.Create(request.UserId);

        var result = basket.UpdateQuantity(request.ProductId, request.Quantity);
        if (result.IsFailure)
            return Result<GetBasketResponse>.Failure(result.Error!);

        await _basketRepository.SaveBasketAsync(basket);

        return Result<GetBasketResponse>.Success(new GetBasketResponse(request.UserId, basket.Items.Select(i => new BasketItemDto(i.ProductId, i.ProductName, i.Price, i.Quantity, i.TotalPrice, i.ImageUrl)).ToList(), basket.TotalAmount));
    }
}
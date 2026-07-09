using MediatR;
using Basket.Application.Queries.GetBasket;
using Basket.Domain;
using Shared.Domain;
using Basket.Application.Models;
using System.Net.Http.Json;
using StackExchange.Redis;

namespace Basket.Application.Commands.AddItemToBasket;

public class AddItemToBasketHandler : IRequestHandler<AddItemToBasketCommand, Result<GetBasketResponse>>
{
    private readonly IBasketRepository _basketRepository;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IDatabase _redisDb;

    public AddItemToBasketHandler(IBasketRepository basketRepository, IHttpClientFactory httpClientFactory, IDatabase redisDb)
    {
        _basketRepository = basketRepository;
        _httpClientFactory = httpClientFactory;
        _redisDb = redisDb;
    }

    public async Task<Result<GetBasketResponse>> Handle(AddItemToBasketCommand request, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"http://catalog-api:8080/api/Catalog/{request.ProductId}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            return Result<GetBasketResponse>.Failure("Товар не найден");

        var product = await response.Content.ReadFromJsonAsync<ProductDto>(cancellationToken);

        if (product is null)
            return Result<GetBasketResponse>.Failure("Товар не найден");

        var key = $"product:{product.Id}:stock";

        _ = _redisDb.StringSetAsync(key, product.StockQuantity, TimeSpan.FromDays(2));

        if (product.StockQuantity < request.Quantity)
            return Result<GetBasketResponse>.Failure("Недостаточно товара на складе");

        var itemResult = BasketItem.Create(product!.Id, product.Name, product.Price, request.Quantity, product.ImageUrl);
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
}
using Catalog.Domain.Events;
using Shared.Domain;

namespace Catalog.Domain;

public class Product : AggregateRoot<int>
{
    public string Name { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal Price { get; private set; }

    private Product() { }

    private Product(string name, decimal price)
    {
        Name = name;
        Price = price;
    }

    public static Result<Product> Create(string name, decimal price)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure("Название товара не может быть пустым");

        var priceResult = ValidatePrice(price);
        if (priceResult.IsFailure)
            return Result<Product>.Failure(priceResult.Error!);

        var product = new Product(name, price);
        product.RaiseDomainEvent(new ProductCreatedDomainEvent(product.Id, name));

        return Result<Product>.Success(product);
    }

    public Result UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result.Failure("Название товара не может быть пустым");

        Name = name;
        return Result.Success();
    }

    public Result UpdatePrice(decimal newPrice)
    {
        var priceResult = ValidatePrice(newPrice);
        if (priceResult.IsFailure)
            return Result.Failure(priceResult.Error!);

        Price = newPrice;
        return Result.Success();
    }

    public void SetImageUrl(string url)
    {
        ImageUrl = url;
    }

    private static Result ValidatePrice(decimal price)
    {
        if (price < 0)
            return Result.Failure("Цена не может быть меньше 0");

        if (Math.Round(price, 2) != price)
            return Result.Failure("Цена не может содержать более двух знаков после запятой");

        return Result.Success();
    }
}
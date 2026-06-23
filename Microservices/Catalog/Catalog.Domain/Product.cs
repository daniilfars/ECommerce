using Catalog.Domain.Events;
using Shared.Domain;

namespace Catalog.Domain;

public class Product : AggregateRoot<int>
{
    private Money _price = null!;

    public string Name { get; private set; }
    public string? ImageUrl { get; private set; }

    public Money Price
    {
        get => _price;
        private set => _price = value;
    }

    private Product() { }

    private Product(string name, Money price)
    {
        Name = name;
        _price = price;
    }

    public static Result<Product> Create(string name, Money price)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Result<Product>.Failure("Название товара не может быть пустым");

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

    public Result UpdatePrice(Money newPrice)
    {
        _price = newPrice;
        return Result.Success();
    }

    public void SetImageUrl(string url)
    {
        ImageUrl = url;
    }
}

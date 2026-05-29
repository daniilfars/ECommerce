using Modules.Catalog.Domain.Events;
using Shared.Domain;

namespace Modules.Catalog.Domain;

public class Product : AggregateRoot<int>
{
    public string Name { get; private set; }
    public decimal PriceAmount { get; private set; }
    public string PriceCurrency { get; private set; } = string.Empty;
    public Money Price => Money.Create(PriceAmount, PriceCurrency).Value!;

    private Product() { } // Для EF Core
    private Product(string name, Money price)
    {
        Name = name;
        PriceAmount = price.Amount;
        PriceCurrency = price.Currency;
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

    public Result UpdatePrice(Money price)
    {
        PriceAmount = price.Amount;
        PriceCurrency = price.Currency;
        return Result.Success();
    }
}

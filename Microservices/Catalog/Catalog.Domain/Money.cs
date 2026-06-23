using Shared.Domain;

namespace Catalog.Domain;

public class Money : ValueObject
{
    public decimal Amount { get; private set; }
    public string Currency { get; private set; }

    private Money(decimal amount, string currency)
    {
        Amount = amount;
        Currency = currency;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        return [Amount, Currency];
    }

    public static Result<Money> Create(decimal amount, string currency)
    {
        if (string.IsNullOrWhiteSpace(currency))
            return Result<Money>.Failure("Название валюты не может быть пустой");

        if (amount < 0)
            return Result<Money>.Failure("Цена не может быть меньше 0");

        if (Math.Round(amount, 2) != amount)
            return Result<Money>.Failure("Цена не может содержать более двух знаков после запятой");

        return Result<Money>.Success(new Money(amount, currency));
    }
}
using FluentAssertions;

namespace Catalog.Domain.Tests;

public class MoneyTests
{
    [Fact]
    public void Create_ValidAmount_ReturnsSuccess()
    {
        var result = Money.Create(100.50m, "RUB");

        result.IsSuccess.Should().BeTrue();
        result.Value!.Amount.Should().Be(100.50m);
        result.Value.Currency.Should().Be("RUB");
    }

    [Fact]
    public void Create_EmptyCurrency_ReturnsFailure()
    {
        var result = Money.Create(100.50m, "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название валюты не может быть пустой");
    }

    [Fact]
    public void Create_NegativeAmount_ReturnsFailure()
    {
        var result = Money.Create(-5, "RUB");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть меньше 0");
    }

    [Fact]
    public void Create_TooManyDecimals_ReturnsFailure()
    {
        var result = Money.Create(100.999m, "RUB");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может содержать более двух знаков после запятой");
    }

    [Fact]
    public void Equals_SameValues_ReturnsTrue()
    {
        var money1 = Money.Create(100, "RUB").Value;
        var money2 = Money.Create(100, "RUB").Value;

        money1.Should().Be(money2);
    }

    [Fact]
    public void Equals_DifferentValues_ReturnsFalse()
    {
        var money1 = Money.Create(100, "RUB").Value;
        var money2 = Money.Create(200, "RUB").Value;

        money1.Should().NotBe(money2);
    }
}
using FluentAssertions;

namespace Catalog.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Create_ValidName_ReturnsSuccess()
    {
        var money = Money.Create(100, "RUB");
        var result = Product.Create("Bear", money.Value!);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Bear");
        result.Value!.Price.Amount.Should().Be(money.Value!.Amount);
        result.Value!.Price.Currency.Should().Be(money.Value!.Currency);
    }

    [Fact]
    public void Create_EmptyName_ReturnsFailure()
    {
        var money = Money.Create(100, "RUB");
        var result = Product.Create("", money.Value!);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void UpdateName_ValidName_ReturnsSuccess()
    {
        var money = Money.Create(100, "RUB");
        var product = Product.Create("Bear", money.Value!).Value!;

        var result = product.UpdateName("Ball");

        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("Ball");
    }

    [Fact]
    public void UpdateName_EmptyName_ReturnsFailure()
    {
        var money = Money.Create(100, "RUB");
        var product = Product.Create("Bear", money.Value!).Value!;

        var result = product.UpdateName("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void UpdatePrice_ValidPrice_ReturnsSuccess()
    {
        var money1 = Money.Create(100, "RUB").Value!;
        var product = Product.Create("Bear", money1).Value!;

        var money2 = Money.Create(111, "DOL").Value!;
        var result = product.UpdatePrice(money2);

        result.IsSuccess.Should().BeTrue();
        product.Price.Currency.Should().Be(money2.Currency);
        product.Price.Amount.Should().Be(money2.Amount);
    }

    [Fact]
    public void SetImageUrl_ValidUrl_SetUrl()
    {
        var money = Money.Create(100, "RUB").Value!;
        var product = Product.Create("Bear", money).Value!;

        product.SetImageUrl("urlImage");

        product.ImageUrl.Should().Be("urlImage");
    }

    [Fact]
    public void SetImageUrl_NullUrl_SetsNull()
    {
        var money = Money.Create(100, "RUB").Value!;
        var product = Product.Create("Bear", money).Value!;

        product.SetImageUrl(null!);

        product.ImageUrl.Should().BeNull();
    }
}
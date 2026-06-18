using FluentAssertions;

namespace Basket.Domain.Tests;

public class BasketItemTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var result = BasketItem.Create(1, "Phone", 999.99m, "RUB", 2, null);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ProductId.Should().Be(1);
        result.Value.ProductName.Should().Be("Phone");
        result.Value.PriceAmount.Should().Be(999.99m);
        result.Value.PriceCurrency.Should().Be("RUB");
        result.Value.Quantity.Should().Be(2);
        result.Value.ImageUrl.Should().BeNull();
        result.Value.TotalPrice.Should().Be(1999.98m);
    }

    [Fact]
    public void Create_WithImageUrl_ReturnsSuccess()
    {
        var result = BasketItem.Create(1, "Phone", 999.99m, "RUB", 1, "http://image.jpg");

        result.Value!.ImageUrl.Should().Be("http://image.jpg");
    }

    [Fact]
    public void Create_ZeroQuantity_ReturnsFailure()
    {
        var result = BasketItem.Create(1, "Phone", 999.99m, "RUB", 0, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество должно быть больше нуля");
    }

    [Fact]
    public void Create_NegativeQuantity_ReturnsFailure()
    {
        var result = BasketItem.Create(1, "Phone", 999.99m, "RUB", -1, null);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Create_NegativePrice_ReturnsFailure()
    {
        var result = BasketItem.Create(1, "Phone", -10m, "RUB", 1, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть отрицательной");
    }

    [Fact]
    public void UpdateQuantity_ValidValue_UpdatesQuantity()
    {
        var item = BasketItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!;

        item.UpdateQuantity(5);

        item.Quantity.Should().Be(5);
        item.TotalPrice.Should().Be(500m);
    }
}
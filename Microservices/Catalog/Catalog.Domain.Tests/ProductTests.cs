using FluentAssertions;

namespace Catalog.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var result = Product.Create("Bear", 100.50m, 10);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Bear");
        result.Value!.Price.Should().Be(100.50m);
        result.Value!.StockQuantity.Should().Be(10);
    }

    [Fact]
    public void Create_EmptyName_ReturnsFailure()
    {
        var result = Product.Create("", 100m, 10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void Create_NegativePrice_ReturnsFailure()
    {
        var result = Product.Create("Bear", -5m, 10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть меньше 0");
    }

    [Fact]
    public void Create_NegativeStockQuantity_ReturnsFailure()
    {
        var result = Product.Create("Bear", 100m, -5);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество товара на складе не может быть отрицательным");
    }

    [Fact]
    public void Create_TooManyDecimals_ReturnsFailure()
    {
        var result = Product.Create("Bear", 100.999m, 10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может содержать более двух знаков после запятой");
    }

    [Fact]
    public void UpdateName_ValidName_ReturnsSuccess()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdateName("Ball");

        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("Ball");
    }

    [Fact]
    public void UpdateName_EmptyName_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdateName("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void UpdatePrice_ValidPrice_ReturnsSuccess()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdatePrice(200m);

        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(200m);
    }

    [Fact]
    public void UpdatePrice_NegativePrice_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdatePrice(-10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть меньше 0");
    }

    [Fact]
    public void UpdateStockQuantity_ValidQuantity_ReturnsSuccess()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdateStockQuantity(50);

        result.IsSuccess.Should().BeTrue();
        product.StockQuantity.Should().Be(50);
    }

    [Fact]
    public void UpdateStockQuantity_NegativeQuantity_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.UpdateStockQuantity(-5);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество товара на складе не может быть отрицательным");
    }

    [Fact]
    public void SetImageUrl_ValidUrl_SetUrl()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        product.SetImageUrl("urlImage");

        product.ImageUrl.Should().Be("urlImage");
    }

    [Fact]
    public void SetImageUrl_NullUrl_SetsNull()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        product.SetImageUrl(null!);

        product.ImageUrl.Should().BeNull();
    }

    [Fact]
    public void ReserveStock_ValidQuantity_ReducesStock()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.ReserveStock(3);

        result.IsSuccess.Should().BeTrue();
        product.StockQuantity.Should().Be(7);
    }

    [Fact]
    public void ReserveStock_ZeroQuantity_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.ReserveStock(0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество для резервирования должно быть больше нуля");
    }

    [Fact]
    public void ReserveStock_NegativeQuantity_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 10).Value!;

        var result = product.ReserveStock(-2);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество для резервирования должно быть больше нуля");
    }

    [Fact]
    public void ReserveStock_MoreThanAvailable_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m, 5).Value!;

        var result = product.ReserveStock(10);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Недостаточно товара на складе");
    }

    [Fact]
    public void ReturnStock_IncreasesStock()
    {
        var product = Product.Create("Bear", 100m, 5).Value!;
        product.ReserveStock(3);

        product.ReturnStock(2);

        product.StockQuantity.Should().Be(4);
    }
}
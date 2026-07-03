using FluentAssertions;

namespace Catalog.Domain.Tests;

public class ProductTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var result = Product.Create("Bear", 100.50m);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Name.Should().Be("Bear");
        result.Value!.Price.Should().Be(100.50m);
    }

    [Fact]
    public void Create_EmptyName_ReturnsFailure()
    {
        var result = Product.Create("", 100m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void Create_NegativePrice_ReturnsFailure()
    {
        var result = Product.Create("Bear", -5m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть меньше 0");
    }

    [Fact]
    public void Create_TooManyDecimals_ReturnsFailure()
    {
        var result = Product.Create("Bear", 100.999m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может содержать более двух знаков после запятой");
    }

    [Fact]
    public void UpdateName_ValidName_ReturnsSuccess()
    {
        var product = Product.Create("Bear", 100m).Value!;

        var result = product.UpdateName("Ball");

        result.IsSuccess.Should().BeTrue();
        product.Name.Should().Be("Ball");
    }

    [Fact]
    public void UpdateName_EmptyName_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m).Value!;

        var result = product.UpdateName("");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Название товара не может быть пустым");
    }

    [Fact]
    public void UpdatePrice_ValidPrice_ReturnsSuccess()
    {
        var product = Product.Create("Bear", 100m).Value!;

        var result = product.UpdatePrice(200m);

        result.IsSuccess.Should().BeTrue();
        product.Price.Should().Be(200m);
    }

    [Fact]
    public void UpdatePrice_NegativePrice_ReturnsFailure()
    {
        var product = Product.Create("Bear", 100m).Value!;

        var result = product.UpdatePrice(-10m);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть меньше 0");
    }

    [Fact]
    public void SetImageUrl_ValidUrl_SetUrl()
    {
        var product = Product.Create("Bear", 100m).Value!;

        product.SetImageUrl("urlImage");

        product.ImageUrl.Should().Be("urlImage");
    }

    [Fact]
    public void SetImageUrl_NullUrl_SetsNull()
    {
        var product = Product.Create("Bear", 100m).Value!;

        product.SetImageUrl(null!);

        product.ImageUrl.Should().BeNull();
    }
}
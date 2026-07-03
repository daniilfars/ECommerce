using FluentAssertions;

namespace Ordering.Domain.Tests;

public class OrderItemTests
{
    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var result = OrderItem.Create(1, "Phone", 999.99m, 2, null);

        result.IsSuccess.Should().BeTrue();
        result.Value!.ProductId.Should().Be(1);
        result.Value.ProductName.Should().Be("Phone");
        result.Value.Price.Should().Be(999.99m);
        result.Value.Quantity.Should().Be(2);
        result.Value.ImageUrl.Should().BeNull();
        result.Value.TotalPrice.Should().Be(1999.98m);
    }

    [Fact]
    public void Create_ZeroQuantity_ReturnsFailure()
    {
        var result = OrderItem.Create(1, "Phone", 100m, 0, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество должно быть больше нуля");
    }

    [Fact]
    public void Create_NegativePrice_ReturnsFailure()
    {
        var result = OrderItem.Create(1, "Phone", -10m, 1, null);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Цена не может быть отрицательной");
    }
}
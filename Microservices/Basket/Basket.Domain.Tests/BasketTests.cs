using FluentAssertions;

namespace Basket.Domain.Tests;

public class BasketTests
{
    [Fact]
    public void Create_ReturnsBasketWithUserId()
    {
        var userId = Guid.NewGuid();
        var basket = Basket.Create(userId);

        basket.UserId.Should().Be(userId);
        basket.Items.Should().BeEmpty();
        basket.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void AddItem_NewProduct_AddsToItems()
    {
        var basket = Basket.Create(Guid.NewGuid());
        var item = BasketItem.Create(1, "Phone", 999.99m, "RUB", 1, null).Value!;

        basket.AddItem(item);

        basket.Items.Should().HaveCount(1);
        basket.Items.First().ProductId.Should().Be(1);
    }

    [Fact]
    public void AddItem_ExistingProduct_IncreasesQuantity()
    {
        var basket = Basket.Create(Guid.NewGuid());
        var item1 = BasketItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!;
        var item2 = BasketItem.Create(1, "Phone", 100m, "RUB", 3, null).Value!;

        basket.AddItem(item1);
        basket.AddItem(item2);

        basket.Items.Should().HaveCount(1);
        basket.Items.First().Quantity.Should().Be(5);
        basket.Items.First().TotalPrice.Should().Be(500m);
    }

    [Fact]
    public void RemoveItem_ExistingProduct_RemovesFromItems()
    {
        var basket = Basket.Create(Guid.NewGuid());
        var item = BasketItem.Create(1, "Phone", 100m, "RUB", 1, null).Value!;
        basket.AddItem(item);

        var result = basket.RemoveItem(1);

        result.IsSuccess.Should().BeTrue();
        basket.Items.Should().BeEmpty();
    }

    [Fact]
    public void RemoveItem_NonExistingProduct_ReturnsFailure()
    {
        var basket = Basket.Create(Guid.NewGuid());

        var result = basket.RemoveItem(999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Товар не найден в корзине");
    }

    [Fact]
    public void UpdateQuantity_ExistingProduct_UpdatesQuantity()
    {
        var basket = Basket.Create(Guid.NewGuid());
        var item = BasketItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!;
        basket.AddItem(item);

        var result = basket.UpdateQuantity(1, 10);

        result.IsSuccess.Should().BeTrue();
        basket.Items.First().Quantity.Should().Be(10);
        basket.Items.First().TotalPrice.Should().Be(1000m);
    }

    [Fact]
    public void UpdateQuantity_NonExistingProduct_ReturnsFailure()
    {
        var basket = Basket.Create(Guid.NewGuid());

        var result = basket.UpdateQuantity(999, 5);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void UpdateQuantity_ZeroQuantity_ReturnsFailure()
    {
        var basket = Basket.Create(Guid.NewGuid());
        var item = BasketItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!;
        basket.AddItem(item);

        var result = basket.UpdateQuantity(1, 0);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Количество должно быть больше нуля");
    }

    [Fact]
    public void Clear_RemovesAllItems()
    {
        var basket = Basket.Create(Guid.NewGuid());
        basket.AddItem(BasketItem.Create(1, "Phone", 100m, "RUB", 1, null).Value!);
        basket.AddItem(BasketItem.Create(2, "Case", 50m, "RUB", 1, null).Value!);

        basket.Clear();

        basket.Items.Should().BeEmpty();
        basket.TotalAmount.Should().Be(0);
    }

    [Fact]
    public void TotalAmount_CalculatesCorrectly()
    {
        var basket = Basket.Create(Guid.NewGuid());
        basket.AddItem(BasketItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!);
        basket.AddItem(BasketItem.Create(2, "Case", 50m, "RUB", 1, null).Value!);

        basket.TotalAmount.Should().Be(250m);
    }
}
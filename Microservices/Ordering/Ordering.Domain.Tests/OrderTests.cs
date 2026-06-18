using FluentAssertions;

namespace Ordering.Domain.Tests;

public class OrderTests
{
    private readonly string _address = "г. Москва, ул. Ленина, д. 1";

    [Fact]
    public void Create_ValidData_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();
        var result = Order.Create(userId, _address);

        result.IsSuccess.Should().BeTrue();
        result.Value!.UserId.Should().Be(userId);
        result.Value.ShippingAddress.Should().Be(_address);
        result.Value.Status.Should().Be(OrderStatus.Pending);
        result.Value.TotalAmount.Should().Be(0);
        result.Value.Items.Should().BeEmpty();
    }

    [Fact]
    public void Create_EmptyAddress_ReturnsFailure()
    {
        var result = Order.Create(Guid.NewGuid(), "");

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Адрес не может быть пустым");
    }

    [Fact]
    public void AddItem_NewItem_AddsToOrderAndRecalculatesTotal()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        var item = OrderItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!;

        order.AddItem(item);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
    }

    [Fact]
    public void AddItem_WhenNotPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();

        var item = OrderItem.Create(1, "Phone", 100m, "RUB", 1, null).Value!;
        var result = order.AddItem(item);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Нельзя изменить заказ в текущем статусе");
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesAndRecalculatesTotal()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.AddItem(OrderItem.Create(1, "Phone", 100m, "RUB", 2, null).Value!);
        order.AddItem(OrderItem.Create(2, "Case", 50m, "RUB", 1, null).Value!);

        var result = order.RemoveItem(1);

        result.IsSuccess.Should().BeTrue();
        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(50m);
    }

    [Fact]
    public void RemoveItem_NonExistingItem_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.RemoveItem(999);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Товар не найден в заказе");
    }

    [Fact]
    public void RemoveItem_WhenNotPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.AddItem(OrderItem.Create(1, "Phone", 100m, "RUB", 1, null).Value!);
        order.Pay();

        var result = order.RemoveItem(1);

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Нельзя изменить заказ в текущем статусе");
    }

    [Fact]
    public void Pay_WhenPending_ChangesStatusToPaid()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Pay();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void Pay_WhenNotPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();

        var result = order.Pay();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Только ожидающий заказ можно оплатить");
    }

    [Fact]
    public void Cancel_WhenPending_ChangesStatusToCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Cancel();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Cancel();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Заказ уже отменён");
    }

    [Fact]
    public void Cancel_WhenShipped_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();
        order.Ship();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Нельзя отменить доставленный заказ");
    }

    [Fact]
    public void Cancel_WhenDelivered_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();
        order.Ship();
        order.Deliver();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Нельзя отменить доставленный заказ");
    }

    [Fact]
    public void Ship_WhenPaid_ChangesStatusToShipped()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();

        var result = order.Ship();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Shipped);
    }

    [Fact]
    public void Ship_WhenNotPaid_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Ship();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Заказ не оплачен");
    }

    [Fact]
    public void Deliver_WhenShipped_ChangesStatusToDelivered()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Pay();
        order.Ship();

        var result = order.Deliver();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void Deliver_WhenNotShipped_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Deliver();

        result.IsFailure.Should().BeTrue();
        result.Error.Should().Be("Только отправленный заказ можно доставить");
    }

    [Fact]
    public void FullLifecycle_AllStatusesAreCorrect()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Status.Should().Be(OrderStatus.Pending);

        order.Pay();
        order.Status.Should().Be(OrderStatus.Paid);

        order.Ship();
        order.Status.Should().Be(OrderStatus.Shipped);

        order.Deliver();
        order.Status.Should().Be(OrderStatus.Delivered);
    }
}
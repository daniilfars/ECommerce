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
    public void Create_WithOrderId_ReturnsSuccess()
    {
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var result = Order.Create(orderId, userId, _address);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(orderId);
        result.Value.UserId.Should().Be(userId);
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
        var item = OrderItem.Create(1, "Phone", 100m, 2, null).Value!;

        order.AddItem(item);

        order.Items.Should().HaveCount(1);
        order.TotalAmount.Should().Be(200m);
    }

    [Fact]
    public void AddItem_WhenNotPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();

        var item = OrderItem.Create(1, "Phone", 100m, 1, null).Value!;
        var result = order.AddItem(item);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void RemoveItem_ExistingItem_RemovesAndRecalculatesTotal()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.AddItem(OrderItem.Create(1, "Phone", 100m, 2, null).Value!);
        order.AddItem(OrderItem.Create(2, "Case", 50m, 1, null).Value!);

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
    }

    [Fact]
    public void RemoveItem_WhenNotPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.AddItem(OrderItem.Create(1, "Phone", 100m, 1, null).Value!);
        order.Confirm();

        var result = order.RemoveItem(1);

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Confirm_WhenPending_ChangesStatusToConfirmed()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Confirm_WhenNotPending_DoesNothing()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();

        order.Confirm();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void RejectDueToNoStock_WhenPending_ChangesStatusToCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        order.RejectDueToNoStock();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void RejectDueToNoStock_WhenNotPending_DoesNothing()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();

        order.RejectDueToNoStock();

        order.Status.Should().Be(OrderStatus.Confirmed);
    }

    [Fact]
    public void Pay_WhenConfirmed_ChangesStatusToPaid()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();

        var result = order.Pay();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Paid);
    }

    [Fact]
    public void Pay_WhenPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Pay();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Pay_WhenAlreadyPaid_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
        order.Pay();

        var result = order.Pay();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Cancel_WhenConfirmed_ChangesStatusToCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();

        var result = order.Cancel();

        result.IsSuccess.Should().BeTrue();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenPending_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Cancel_AlreadyCancelled_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
        order.Cancel();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Cancel_WhenShipped_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
        order.Pay();
        order.Ship();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Cancel_WhenDelivered_ReturnsFailure()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
        order.Pay();
        order.Ship();
        order.Deliver();

        var result = order.Cancel();

        result.IsFailure.Should().BeTrue();
    }

    [Fact]
    public void Ship_WhenPaid_ChangesStatusToShipped()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
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
    }

    [Fact]
    public void Deliver_WhenShipped_ChangesStatusToDelivered()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Confirm();
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
    }

    [Fact]
    public void FullLifecycle_AllStatusesAreCorrect()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Status.Should().Be(OrderStatus.Pending);

        order.Confirm();
        order.Status.Should().Be(OrderStatus.Confirmed);

        order.Pay();
        order.Status.Should().Be(OrderStatus.Paid);

        order.Ship();
        order.Status.Should().Be(OrderStatus.Shipped);

        order.Deliver();
        order.Status.Should().Be(OrderStatus.Delivered);
    }

    [Fact]
    public void RejectDueToNoStock_Lifecycle()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;
        order.Status.Should().Be(OrderStatus.Pending);

        order.RejectDueToNoStock();
        order.Status.Should().Be(OrderStatus.Cancelled);
    }

    [Fact]
    public void ForceCancelDueToNoStock_WhenPending_ChangesStatusToCancelled()
    {
        var order = Order.Create(Guid.NewGuid(), _address).Value!;

        order.RejectDueToNoStock();

        order.Status.Should().Be(OrderStatus.Cancelled);
    }
}
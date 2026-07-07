using Shared.Domain;
using Ordering.Domain.Events;

namespace Ordering.Domain;

public class Order : AggregateRoot<Guid>
{
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal TotalAmount { get; private set; }
    public string? PaymentId { get; private set; }

    private Order() { }// Для EF Core
    private Order(Guid id, Guid userId, string shippingAddress)
    {
        Id = id;
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
        PaymentId = null;
    }

    public static Result<Order> Create(Guid userId, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            return Result<Order>.Failure("Адрес не может быть пустым");

        var order = new Order(Guid.NewGuid(), userId, shippingAddress);
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, userId));

        return Result<Order>.Success(order);
    }

    // Этот метод будет вызывать консьюмер(createOrderHandler), который получает OrderId от продюсера(CheckoutBasketHandler)
    public static Result<Order> Create(Guid orderId, Guid userId, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            return Result<Order>.Failure("Адрес не может быть пустым");

        var order = new Order(orderId, userId, shippingAddress);
        order.RaiseDomainEvent(new OrderCreatedDomainEvent(order.Id, userId));

        return Result<Order>.Success(order);
    }

    public Result AddItem(OrderItem item)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure("Нельзя изменить заказ в текущем статусе");

        _items.Add(item);
        RecalculateTotal();
        return Result.Success();
    }

    public Result RemoveItem(int productId)
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure("Нельзя изменить заказ в текущем статусе");

        var item = _items.FirstOrDefault(i => i.ProductId == productId);
        if(item is null)
            return Result.Failure("Товар не найден в заказе");

        _items.Remove(item);
        RecalculateTotal();
        return Result.Success();
    }

    public Result SetPaymentId(string paymentId)
    {
        PaymentId = paymentId;
        return Result.Success();
    }

    public Result Cancel()
    {
        if (Status == OrderStatus.Pending)
            return Result.Failure("Нельзя отменить заказ, пока он проверяется на складе");

        if (Status == OrderStatus.Cancelled)
            return Result.Failure("Заказ уже отменён");

        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            return Result.Failure("Нельзя отменить доставленный заказ");

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public void RejectDueToNoStock()
    {
        if (Status == OrderStatus.Pending)
            Status = OrderStatus.Cancelled;
    }

    public void Confirm()
    {
        if (Status == OrderStatus.Pending)
            Status = OrderStatus.Confirmed;
    }

    public Result Pay()
    {
        if (Status != OrderStatus.Confirmed)
            return Result.Failure("Только подтвержденный заказ можно оплатить");

        Status = OrderStatus.Paid;
        return Result.Success();
    }

    public Result Ship()
    {
        if (Status != OrderStatus.Paid)
            return Result.Failure("Заказ не оплачен");

        Status = OrderStatus.Shipped;
        return Result.Success();
    }

    public Result Deliver()
    {
        if (Status != OrderStatus.Shipped)
            return Result.Failure("Только отправленный заказ можно доставить");

        Status = OrderStatus.Delivered;
        return Result.Success();
    }

    private void RecalculateTotal()
    {
        TotalAmount = _items.Sum(i => i.TotalPrice);
    }
}
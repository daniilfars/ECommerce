using Shared.Domain;
using Modules.Ordering.Domain.Events;

namespace Modules.Ordering.Domain;

public class Order : AggregateRoot<int>
{
    private readonly List<OrderItem> _items = [];
    public IReadOnlyCollection<OrderItem> Items => _items.AsReadOnly();
    public Guid UserId { get; private set; }
    public OrderStatus Status { get; private set; }
    public string ShippingAddress { get; private set; }
    public decimal TotalAmount { get; private set; }

    private Order() { }// Для EF Core
    private Order(Guid userId, string shippingAddress)
    {
        UserId = userId;
        ShippingAddress = shippingAddress;
        Status = OrderStatus.Pending;
        TotalAmount = 0;
    }

    public static Result<Order> Create(Guid userId, string shippingAddress)
    {
        if (string.IsNullOrWhiteSpace(shippingAddress))
            return Result<Order>.Failure("Адрес не может быть пустым");

        var order = new Order(userId, shippingAddress);
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

    public Result Cancel()
    {
        if (Status == OrderStatus.Cancelled)
            return Result.Failure("Заказ уже отменён");

        if (Status == OrderStatus.Shipped || Status == OrderStatus.Delivered)
            return Result.Failure("Нельзя отменить доставленный заказ");

        Status = OrderStatus.Cancelled;
        return Result.Success();
    }

    public Result Pay()
    {
        if (Status != OrderStatus.Pending)
            return Result.Failure("Только ожидающий заказ можно оплатить");

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
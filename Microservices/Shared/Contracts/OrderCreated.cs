namespace Shared.Contracts;

public interface OrderCreated
{
    Guid OrderId { get; }
    Guid UserId { get; }
    string ShippingAddress { get; }
    decimal TotalAmount { get; }
    OrderItemData[] Items { get; }
}
public interface OrderItemData
{
    int ProductId { get; }
    string ProductName { get; }
    decimal Price { get; }
    int Quantity { get; }
}
namespace Shared.Contracts;

public interface StockReserveRequested
{
    Guid OrderId { get; }
    ProductQuantity[] Items { get; }
}

public interface ProductQuantity
{
    int ProductId { get; }
    int Quantity { get; }
}
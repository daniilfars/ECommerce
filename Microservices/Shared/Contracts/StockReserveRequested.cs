namespace Shared.Contracts;

public interface StockReserveRequested
{
    int OrderId { get; }
    ProductQuantity[] Items { get; }
}

public interface ProductQuantity
{
    int ProductId { get; }
    int Quantity { get; }
}
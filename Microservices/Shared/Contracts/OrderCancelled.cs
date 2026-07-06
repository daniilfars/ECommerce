namespace Shared.Contracts;

public interface OrderCancelled
{
    ProductQuantity[] Items { get; }
}
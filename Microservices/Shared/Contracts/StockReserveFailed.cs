namespace Shared.Contracts;

public interface StockReserveFailed
{
    Guid OrderId { get; }
    string Reason { get; }
}
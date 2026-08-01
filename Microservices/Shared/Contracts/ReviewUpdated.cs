namespace Shared.Contracts;

public interface ReviewUpdated
{
    int ProductId { get; }
    int DifferenceStars { get; }
}
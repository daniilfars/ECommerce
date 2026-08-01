namespace Shared.Contracts;

public interface ReviewCreated
{
    int ProductId { get; }
    int Stars { get; }
}
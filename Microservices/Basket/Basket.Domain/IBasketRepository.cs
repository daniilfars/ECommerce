namespace Basket.Domain;

public interface IBasketRepository
{
    Task<Basket?> GetBasketAsync(Guid userId);
    Task SaveBasketAsync(Basket basket);
    Task DeleteAsync(Guid userId);
}
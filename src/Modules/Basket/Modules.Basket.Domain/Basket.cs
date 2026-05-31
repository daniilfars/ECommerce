using Shared.Domain;

namespace Modules.Basket.Domain;

public class Basket
{
    public Guid UserId { get; set; } // public set для сериализации
    public List<BasketItem> Items { get; set; } = [];

    public Basket() { } // Для десериализации
    private Basket(Guid userId)
    {
        UserId = userId;
    }

    public static Basket Create(Guid userId)
    {
        return new Basket(userId);
    }

    public void AddItem(BasketItem item)
    {
        var existing = Items.FirstOrDefault(i => i.ProductId == item.ProductId);
        if (existing is not null)
        {
            existing.UpdateQuantity(existing.Quantity + item.Quantity);
            return;
        }
        Items.Add(item);
    }

    public Result RemoveItem(int productId)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure("Товар не найден в корзине");

        Items.Remove(item);
        return Result.Success();
    }

    public Result UpdateQuantity(int productId, int newQuantity)
    {
        var item = Items.FirstOrDefault(i => i.ProductId == productId);
        if (item is null)
            return Result.Failure("Товар не найден в корзине");

        if (newQuantity <= 0)
            return Result.Failure("Количество должно быть больше нуля");

        item.UpdateQuantity(newQuantity);
        return Result.Success();
    }

    public void Clear()
    {
        Items.Clear();
    }

    public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
}
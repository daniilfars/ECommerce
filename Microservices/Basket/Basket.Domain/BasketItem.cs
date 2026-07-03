using Shared.Domain;

namespace Basket.Domain;

public class BasketItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public string? ImageUrl { get; set; }
    public decimal TotalPrice => Price * Quantity;

    public BasketItem() { } // Для десериализации

    private BasketItem(int productId, string name, decimal price, int quantity, string? imageUrl)
    {
        ProductId = productId;
        ProductName = name;
        Price = price;
        Quantity = quantity;
        ImageUrl = imageUrl;
    }

    public static Result<BasketItem> Create(int productId, string name, decimal price, int quantity, string? imageUrl)
    {
        if (quantity <= 0)
            return Result<BasketItem>.Failure("Количество должно быть больше нуля");
        if (price < 0)
            return Result<BasketItem>.Failure("Цена не может быть отрицательной");

        return Result<BasketItem>.Success(new BasketItem(productId, name, price, quantity, imageUrl));
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
    }
}
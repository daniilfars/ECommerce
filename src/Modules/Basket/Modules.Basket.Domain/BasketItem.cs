using Shared.Domain;

namespace Modules.Basket.Domain;

public class BasketItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal PriceAmount { get; set; }
    public string PriceCurrency { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal TotalPrice => PriceAmount * Quantity;

    public BasketItem() { } // Для десериализации

    private BasketItem(int productId, string name, decimal price, string currency, int quantity)
    {
        ProductId = productId;
        ProductName = name;
        PriceAmount = price;
        PriceCurrency = currency;
        Quantity = quantity;
    }

    public static Result<BasketItem> Create(int productId, string name, decimal price, string currency, int quantity)
    {
        if (quantity <= 0)
            return Result<BasketItem>.Failure("Количество должно быть больше нуля");
        if (price < 0)
            return Result<BasketItem>.Failure("Цена не может быть отрицательной");

        return Result<BasketItem>.Success(new BasketItem(productId, name, price, currency, quantity));
    }

    public void UpdateQuantity(int newQuantity)
    {
        Quantity = newQuantity;
    }
}
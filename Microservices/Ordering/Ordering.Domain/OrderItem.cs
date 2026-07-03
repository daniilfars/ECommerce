using Shared.Domain;

namespace Ordering.Domain;

public class OrderItem
{
    public int Id { get; private set; }
    public int ProductId { get; private set; }
    public string ProductName { get; private set; }
    public decimal Price { get; private set; }
    public int Quantity { get; private set; }
    public string? ImageUrl { get; private set; }
    public decimal TotalPrice => Price * Quantity;

    private OrderItem() { } // Для EF Core

    private OrderItem(int productId, string name, decimal price, int quantity, string? imageUrl)
    {
        ProductId = productId;
        ProductName = name;
        Price = price;
        Quantity = quantity;
        ImageUrl = imageUrl;
    }

    public static Result<OrderItem> Create(int productId, string name, decimal price, int quantity, string? imageUrl)
    {
        if (quantity <= 0)
            return Result<OrderItem>.Failure("Количество должно быть больше нуля");

        if (price < 0)
            return Result<OrderItem>.Failure("Цена не может быть отрицательной");

        return Result<OrderItem>.Success(new OrderItem(productId, name, price, quantity, imageUrl));
    }
}
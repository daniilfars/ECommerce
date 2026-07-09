namespace Shared.Contracts;

public interface ProductsStockChanged
{
    ProductStockInfo[] Products { get; }
}

public interface ProductStockInfo
{
    int ProductId { get; }
    int StockQuantity { get; }
}
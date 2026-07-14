namespace Catalog.Application.Interfaces;

public interface ICatalogCacheService
{
    Task ClearProductByIdAsync(int productId);
    Task ClearCatalogPagesAsync();
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Catalog.Domain;

namespace Catalog.Application.Interfaces;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
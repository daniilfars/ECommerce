using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Catalog.Domain;

namespace Modules.Catalog.Application.Interfaces;

public interface ICatalogDbContext
{
    DbSet<Product> Products { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
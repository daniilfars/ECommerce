using Microsoft.EntityFrameworkCore;
using Modules.Catalog.Application.Interfaces;
using Modules.Catalog.Domain;
using Modules.Catalog.Infrastructure.Configurations;

namespace Modules.Catalog.Infrastructure.Data;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public DbSet<Product> Products { get; set; }

    public CatalogDbContext() { }
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ProductConfiguration());
    }
}

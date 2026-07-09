using Catalog.Application.Interfaces;
using Catalog.Domain;
using Catalog.Infrastructure.Configurations;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.Data;

public class CatalogDbContext : DbContext, ICatalogDbContext
{
    public DbSet<Product> Products { get; set; }

    public CatalogDbContext() { }
    public CatalogDbContext(DbContextOptions<CatalogDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ProductConfiguration());
        builder.AddTransactionalOutboxEntities();
    }
}
using Microsoft.EntityFrameworkCore;
using Modules.Ordering.Application.Interfaces;
using Modules.Ordering.Domain;
using Modules.Ordering.Infrastructure.Configurations;

namespace Modules.Ordering.Infrastructure.Data;

public class OrderingDbContext : DbContext, IOrderingDbContext
{
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> Items { get; set; }
    public OrderingDbContext() { }
    public OrderingDbContext(DbContextOptions<OrderingDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasPostgresEnum<OrderStatus>();
        builder.ApplyConfiguration(new OrderConfiguration());
        builder.ApplyConfiguration(new OrderItemConfiguration());
    }
}
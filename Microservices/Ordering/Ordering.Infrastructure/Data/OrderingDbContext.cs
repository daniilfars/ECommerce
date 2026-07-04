using MassTransit;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Interfaces;
using Ordering.Domain;
using Ordering.Infrastructure.Configurations;

namespace Ordering.Infrastructure.Data;

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
        builder.AddTransactionalOutboxEntities();
    }
}
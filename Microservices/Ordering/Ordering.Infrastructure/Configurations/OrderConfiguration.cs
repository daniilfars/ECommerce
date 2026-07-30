using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain;

namespace Ordering.Infrastructure.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => new { o.UserId, o.Status }).HasFilter($"\"Status\" = {(int)OrderStatus.Delivered}"); // индекс для быстрой работы OrderingGrpcService

        builder.Property(o => o.UserId).IsRequired();
        builder.Property(o => o.Status).IsRequired();
        builder.Property(o => o.ShippingAddress).IsRequired().HasMaxLength(500);
        builder.Property(o => o.TotalAmount).HasPrecision(18, 2).IsRequired();

        builder.HasMany(o => o.Items).WithOne().HasForeignKey("OrderId").OnDelete(DeleteBehavior.Cascade);
    }
}

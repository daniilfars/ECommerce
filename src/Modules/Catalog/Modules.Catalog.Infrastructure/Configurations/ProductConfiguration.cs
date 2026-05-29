using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Modules.Catalog.Domain;

namespace Modules.Catalog.Infrastructure.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        //builder.Property(p => p.Id).ValueGeneratedOnAdd();

        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

        builder.Property(p => p.PriceAmount)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.PriceCurrency)
            .HasMaxLength(3)
            .IsRequired();

        builder.Ignore(p => p.Price);
        builder.Ignore(p => p.DomainEvents);
    }
}
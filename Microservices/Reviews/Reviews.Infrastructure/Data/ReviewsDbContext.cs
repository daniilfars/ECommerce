using Microsoft.EntityFrameworkCore;
using MassTransit;
using Reviews.Application.Interfaces;
using Reviews.Domain;
using Reviews.Infrastructure.Configurations;

namespace Reviews.Infrastructure.Data;

public class ReviewsDbContext : DbContext, IReviewsDbContext
{
    public DbSet<Review> Reviews { get; set; }

    public ReviewsDbContext() { }
    public ReviewsDbContext(DbContextOptions<ReviewsDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new ReviewConfiguration());
        builder.AddTransactionalOutboxEntities();
    }
}
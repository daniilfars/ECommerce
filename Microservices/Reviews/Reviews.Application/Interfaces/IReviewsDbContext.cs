using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Reviews.Domain;

namespace Reviews.Application.Interfaces;

public interface IReviewsDbContext
{
    DbSet<Review> Reviews { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
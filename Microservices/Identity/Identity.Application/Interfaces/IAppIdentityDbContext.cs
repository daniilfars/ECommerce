using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Identity.Domain;

namespace Identity.Application.Interfaces;

public interface IAppIdentityDbContext
{
    DbSet<User> Users { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

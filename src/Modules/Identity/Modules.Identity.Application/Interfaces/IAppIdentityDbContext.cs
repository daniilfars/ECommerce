using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Identity.Domain;

namespace Modules.Identity.Application.Interfaces;

public interface IAppIdentityDbContext
{
    DbSet<User> Users { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

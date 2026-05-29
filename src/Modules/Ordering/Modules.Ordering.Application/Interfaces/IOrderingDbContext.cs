using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Modules.Ordering.Domain;

namespace Modules.Ordering.Application.Interfaces;

public interface IOrderingDbContext
{
    DbSet<Order> Orders { get; }
    DatabaseFacade Database { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
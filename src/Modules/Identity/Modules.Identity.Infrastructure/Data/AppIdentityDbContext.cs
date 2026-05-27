using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Modules.Identity.Application.Interfaces;
using Modules.Identity.Domain;
using Modules.Identity.Infrastructure.Configurations;

namespace Modules.Identity.Infrastructure.Data;

public class AppIdentityDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>, IAppIdentityDbContext
{
    public new DbSet<User> Users { get; set; }

    public AppIdentityDbContext() { }
    public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfiguration(new UserConfiguration());
    }
}
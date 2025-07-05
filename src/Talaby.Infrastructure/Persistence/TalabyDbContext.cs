using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Talaby.Domain.Entities;
using Talaby.Domain.Entities.Projects;

namespace Talaby.Infrastructure.Persistence;

public class TalabyDbContext : IdentityDbContext<AppUser, AppRole, Guid>
{
    public TalabyDbContext(DbContextOptions<TalabyDbContext> options)
        : base(options) { }

    public DbSet<StoreCategory> StoreCategories { get; set; }
    public DbSet<ProjectRequest> ProjectRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); 
        builder.ApplyConfigurationsFromAssembly(typeof(TalabyDbContext).Assembly);


    }
}



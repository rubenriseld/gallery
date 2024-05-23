using Gallery.API.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Database;

public class GalleryDbContext : IdentityDbContext<IdentityUser>
{
    public GalleryDbContext(DbContextOptions<GalleryDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplyConfigurations(modelBuilder);
    }

    public ModelBuilder ApplyConfigurations(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(GalleryDbContext).Assembly);
        return modelBuilder;
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var changes = ChangeTracker.Entries();

        foreach (var item in changes)
        {
            if (item.State == EntityState.Added)
            {
                if (item.Entity is IHasTimeStamps x)
                {
                    if (x.CreatedDate == DateTime.MinValue)
                    {
                        x.CreatedDate = DateTime.UtcNow;
                    }
                }
            }
            if (item.State == EntityState.Modified)
            {
                if (item.Entity is IHasTimeStamps x)
                {
                    x.ModifiedDate = DateTime.UtcNow;
                }
            }
        }

        return base.SaveChangesAsync();
    }
}

using Gallery.Database.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
}

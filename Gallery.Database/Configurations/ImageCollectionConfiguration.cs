using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallery.Database.Configurations;

public class ImageCollectionConfiguration : IEntityTypeConfiguration<ImageCollection>
{
    public void Configure(EntityTypeBuilder<ImageCollection> entity)
    {
        entity.Property(e => e.Name).HasMaxLength(64).IsRequired();
        entity.Property(e => e.Description).HasMaxLength(256);
    }
}

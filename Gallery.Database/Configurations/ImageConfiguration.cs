using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallery.Database.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> entity)
    {
        entity.Property(e => e.Title).HasMaxLength(64);
        entity.Property(e => e.Description).HasMaxLength(256);
        entity.Property(e => e.ImageCollectionId);
    }
}

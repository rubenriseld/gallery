using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Gallery.Database.Configurations;

public class ImageConfiguration : IEntityTypeConfiguration<Image>
{
    public void Configure(EntityTypeBuilder<Image> entity)
    {
        entity.HasMany(i => i.Tags)
            .WithMany(t => t.Images);

        entity.Property(e => e.Title).HasMaxLength(64).IsRequired();
        entity.Property(e => e.Description).HasMaxLength(256);
        entity.Property(e => e.ImageCollectionId).IsRequired();
    }
}

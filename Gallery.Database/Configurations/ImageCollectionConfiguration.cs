using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Gallery.Database.Configurations;

public class ImageCollectionConfiguration : IEntityTypeConfiguration<ImageCollection>
{
    public void Configure(EntityTypeBuilder<ImageCollection> entity)
    {
        entity.HasOne(ic => ic.CoverImage)
            .WithOne(img => img.CoverImageCollection)
            .HasForeignKey<ImageCollection>(ic => ic.CoverImageId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.HasMany(ic => ic.Images)
            .WithOne(img => img.ImageCollection)
            .HasForeignKey(img => img.ImageCollectionId)
            .OnDelete(DeleteBehavior.SetNull);

        entity.Property(e => e.Name).HasMaxLength(64).IsRequired();
        entity.Property(e => e.Description).HasMaxLength(256);
    }
}

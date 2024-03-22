using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Reflection.Emit;

namespace Gallery.Database.Configurations;

public class ImageTagConfiguration : IEntityTypeConfiguration<ImageTag>
{
    public void Configure(EntityTypeBuilder<ImageTag> entity)
    {
        entity.HasKey(e => new { e.ImageId, e.TagId });
    }
}

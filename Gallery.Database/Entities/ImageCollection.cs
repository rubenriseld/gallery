namespace Gallery.Database.Entities;

public class ImageCollection
{
    public string ImageCollectionId { get; private set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? CoverImageId { get; set; }
    public virtual Image? CoverImage { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
}

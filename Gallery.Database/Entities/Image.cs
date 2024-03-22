namespace Gallery.Database.Entities;

public class Image
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string ImageCollectionId { get; set; }
    public virtual ImageCollection? ImageCollection { get; set; }
    public virtual ICollection<ImageTag>? Tags { get; set; }
}

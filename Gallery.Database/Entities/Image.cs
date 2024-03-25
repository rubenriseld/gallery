namespace Gallery.Database.Entities;

public class Image
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public  string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageCollectionId { get; set; }
    public virtual ImageCollection? ImageCollection { get; set; }
    public virtual ICollection<ImageTag>? Tags { get; set; }
}

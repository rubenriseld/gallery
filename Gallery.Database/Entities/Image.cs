namespace Gallery.Database.Entities;

public class Image
{
    public string ImageId { get; private set; } = Guid.NewGuid().ToString();
    public required string Uri { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageCollectionId { get; set; }
    public virtual ImageCollection? ImageCollection { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
} 

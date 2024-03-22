namespace Gallery.Database.Entities;

public class ImageCollection
{
    public string Id { get; private set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<Image>? Images { get; set; }
}

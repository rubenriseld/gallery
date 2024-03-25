namespace Gallery.Database.Entities;

public class ImageTag
{
    public required string ImageId { get; set; }
    public virtual Image Image { get; set; } = null!;
    public required string TagId { get; set; }
    public virtual Tag Tag { get; set; } = null!;
}

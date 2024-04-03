namespace Gallery.Database.Entities;

public class Tag
{
    public string TagId { get; private set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
}

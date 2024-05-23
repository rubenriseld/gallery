using Gallery.API.Interfaces;

namespace Gallery.Database.Entities;

public class Tag : IHasTimeStamps
{
    public string TagId { get; private set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

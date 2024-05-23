using Gallery.API.Interfaces;

namespace Gallery.Database.Entities;

public class ImageCollection : IHasTimeStamps
{
    public string ImageCollectionId { get; private set; } = Guid.NewGuid().ToString();
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? CoverImageId { get; set; }
    public virtual Image? CoverImage { get; set; }
    public virtual ICollection<Image> Images { get; set; } = new HashSet<Image>();
    public bool ShouldBeDisplayed { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}

using Gallery.API.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Gallery.Database.Entities;

public class Image : IHasTimeStamps
{
    public string ImageId { get; private set; } = Guid.NewGuid().ToString();
    public required string Uri { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageCollectionId { get; set; }
    public virtual ImageCollection? ImageCollection { get; set; }
    [Range(0, int.MaxValue)]
    public int OrderInImageCollection { get; set; }
    public bool Sold { get; set; }
    public string? CoverImageCollectionId { get; set; }
    public virtual ImageCollection? CoverImageCollection { get; set; }
    public virtual ICollection<Tag> Tags { get; set; } = new HashSet<Tag>();
    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
} 

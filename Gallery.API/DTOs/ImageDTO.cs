namespace Gallery.API.DTOs;

public record CreateImageDTO
{
    public required string Uri { get; set; }
    public string? Title { get; set; }

}
public record ReadImageDTO
{
    public required string ImageId { get; set; }
    public required string Uri { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? ImageCollectionId { get; set; }
    public string? ImageCollectionName { get; set; }
    public int OrderInImageCollection { get; set; }
    public bool Sold { get; set; }
    public List<ReadTagDTO> Tags { get; set; } = [];
}

public record UpdateImageDTO
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public string? ImageCollectionId { get; set; }
    public bool Sold { get; set; }
    public string[] TagIds { get; set; } = [];
}

public record ReorderImageDTO
{
    public required string ImageId { get; set; }
    public int OrderInImageCollection { get; set; }
}
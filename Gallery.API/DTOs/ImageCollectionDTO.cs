namespace Gallery.API.DTOs;

public class CreateImageCollectionDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
public class ReadImageCollectionDTO
{
    public required string ImageCollectionId { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<ReadImageDTO> Images { get; set; } = [];
}
public class UpdateImageCollectionDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
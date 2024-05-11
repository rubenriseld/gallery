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
    public ReadImageDTO? CoverImage { get; set; }
    public List<ReadImageDTO> Images { get; set; } = [];
}
public class UpdateImageCollectionDTO
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public string? CoverImageId { get; set; }
    public List<ReorderImageDTO> ReorderImages { get; set; } = [];
}
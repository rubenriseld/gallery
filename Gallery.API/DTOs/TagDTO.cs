namespace Gallery.API.DTOs;

public record CreateTagDTO
{
    public required string Name { get; set; }
}
public record ReadTagDTO
{
    public required string TagId { get; set; }
    public required string Name { get; set; }
}
public record UpdateTagDTO
{
    public required string TagId { get; set; }
    public required string Name { get; set; }
}
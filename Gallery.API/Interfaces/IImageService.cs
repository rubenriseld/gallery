using Gallery.API.DTOs;

namespace Gallery.API.Interfaces;

public interface IImageService
{
    Task<List<ReadImageDTO>> UploadImages(IFormFileCollection images);
    Task<List<ReadImageDTO>> GetAllImages();
    Task<ReadImageDTO> UpdateImage(string imageId, UpdateImageDTO updateImageDto);
    Task DeleteImage(string imageId);
}
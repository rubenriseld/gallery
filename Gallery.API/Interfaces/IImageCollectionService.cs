using Gallery.API.DTOs;

namespace Gallery.API.Interfaces;

public interface IImageCollectionService
{
    Task<ReadImageCollectionDTO> CreateImageCollection(CreateImageCollectionDTO createImageCollectionDto);
    Task DeleteImageCollection(string imageCollectionId);
    Task<List<ReadImageCollectionDTO>> GetAllImageCollections();
    Task<ReadImageCollectionDTO> GetImageCollectionById(string imageCollectionId);
    Task<ReadImageCollectionDTO> UpdateImageCollection(string imageCollectionId, UpdateImageCollectionDTO updateImageCollectionDto);
}
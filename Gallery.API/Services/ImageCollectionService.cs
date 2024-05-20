using AutoMapper;
using Gallery.API.DTOs;
using Gallery.API.Interfaces;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Gallery.API.Services;

public class ImageCollectionService : IImageCollectionService
{
    private readonly IRepository<ImageCollection> _imageCollectionRepository;
    private readonly IMapper _mapper;
    public ImageCollectionService(
        IRepository<ImageCollection> imageCollectionRepository, IMapper mapper) =>
        (_imageCollectionRepository, _mapper) = (imageCollectionRepository, mapper);

    public async Task<ReadImageCollectionDTO> CreateImageCollection(CreateImageCollectionDTO createImageCollectionDto)
    {
        var imageCollectionEntity = _mapper.Map<ImageCollection>(createImageCollectionDto);
        await _imageCollectionRepository.Add(imageCollectionEntity);
        await _imageCollectionRepository.SaveChanges();

        return _mapper.Map<ReadImageCollectionDTO>(imageCollectionEntity);
    }

    public async Task<List<ReadImageCollectionDTO>> GetAllImageCollections()
    {
        var imageCollections = await _imageCollectionRepository.Get()
            .Include(c => c.Images.OrderBy(i => i.OrderInImageCollection))
            .ToListAsync();

        return _mapper.Map<List<ReadImageCollectionDTO>>(imageCollections);
    }

    public async Task<ReadImageCollectionDTO> GetImageCollectionById(string imageCollectionId)
    {
        var imageCollection = await _imageCollectionRepository.Get()
            .Where(c => c.ImageCollectionId.Equals(imageCollectionId))
            .Include(c => c.Images.OrderBy(i => i.OrderInImageCollection))
            .ThenInclude(i => i.Tags)
            .SingleOrDefaultAsync()
             ?? throw new KeyNotFoundException($"The entity: {typeof(ImageCollection)} with {nameof(imageCollectionId)}: {imageCollectionId} could not be found."); ;

        return _mapper.Map<ReadImageCollectionDTO>(imageCollection);
    }

    public async Task<ReadImageCollectionDTO> UpdateImageCollection(string imageCollectionId, UpdateImageCollectionDTO updateImageCollectionDto)
    {
        var shouldReorderImages = !updateImageCollectionDto.ReorderImages.IsNullOrEmpty();
        var imageCollection = (shouldReorderImages ?
            await _imageCollectionRepository.Get()
                .Where(i => i.ImageCollectionId.Equals(imageCollectionId))
                .Include(i => i.Images)
                .SingleOrDefaultAsync()
            : await _imageCollectionRepository.Find(imageCollectionId)) ?? throw new KeyNotFoundException($"The entity: {typeof(ImageCollection)} with {nameof(imageCollectionId)}: {imageCollectionId} could not be found.");
        if (shouldReorderImages)
        {
            foreach (var image in imageCollection.Images)
            {
                var reorderImageDTO = updateImageCollectionDto.ReorderImages.FirstOrDefault(reorderImage => reorderImage.ImageId == image.ImageId);
                if (reorderImageDTO != null)
                {
                    image.OrderInImageCollection = reorderImageDTO.OrderInImageCollection;
                }
            }
        }
        _mapper.Map(updateImageCollectionDto, imageCollection);
        await _imageCollectionRepository.SaveChanges();

        return _mapper.Map<ReadImageCollectionDTO>(imageCollection);
    }

    public async Task DeleteImageCollection(string imageCollectionId)
    {
        var imageCollection = await _imageCollectionRepository.Find(imageCollectionId);
        _imageCollectionRepository.Delete(imageCollection);
        await _imageCollectionRepository.SaveChanges();
    }
}

using AutoMapper;
using Gallery.API.DTOs;
using Gallery.API.Interfaces;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

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
        var imageCollections = await _imageCollectionRepository.Get().ToListAsync();

        return _mapper.Map<List<ReadImageCollectionDTO>>(imageCollections);
    }

    public async Task<ReadImageCollectionDTO> GetImageCollectionById(string imageCollectionId)
    {
        var imageCollection = await _imageCollectionRepository.Get()
            .Where(c => c.ImageCollectionId.Equals(imageCollectionId))
            .Include(c => c.Images)
            .ThenInclude(i => i.Tags)
            .SingleOrDefaultAsync();

        return _mapper.Map<ReadImageCollectionDTO>(imageCollection);
    }

    public async Task<ReadImageCollectionDTO> UpdateImageCollection(UpdateImageCollectionDTO updateImageCollectionDto)
    {
        var imageCollection = await _imageCollectionRepository.Find(updateImageCollectionDto.ImageCollectionId);

        _mapper.Map(updateImageCollectionDto, imageCollection);
        _imageCollectionRepository.Update(imageCollection);
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

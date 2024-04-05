using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Gallery.API.DTOs;
using Gallery.API.Interfaces;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Gallery.API.Services;

public class ImageService : IImageService
{
    private readonly IRepository<Image> _imageRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IMapper _mapper;
    private readonly string _storageAccountName;
    private readonly string _storageAccountKey;
    private readonly BlobContainerClient _blobContainerClient;

    public ImageService(
        IConfiguration configuration,
        IRepository<Image> imageRepository,
        IRepository<Tag> tagRepository,
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _tagRepository = tagRepository;
        _mapper = mapper;

        _storageAccountName = configuration["AzureBlobStorage:AccountName"]!;
        _storageAccountKey = configuration["AzureBlobStorage:AccountKey"]!;

        var credentials = new StorageSharedKeyCredential(_storageAccountName, _storageAccountKey);
        var blobUri = $"https://{_storageAccountName}.blob.core.windows.net";
        var blobServiceClient = new BlobServiceClient(new Uri(blobUri), credentials);
        _blobContainerClient = blobServiceClient.GetBlobContainerClient("gallery");
    }

    
    public async Task<List<ReadImageDTO>> UploadImages(
        [FileExtensions(Extensions = "jpg, png, webp", ErrorMessage = "Fileformat not supported.")] 
        IFormFileCollection images)
    {
        var result = new List<ReadImageDTO>();

        foreach (var image in images)
        {
            BlobClient client = _blobContainerClient.GetBlobClient(image.FileName);

            await using (Stream data = image.OpenReadStream())
            {
                await client.UploadAsync(data);
            }

            var imageEntity = _mapper.Map<Image>(new CreateImageDTO { Uri = client.Uri.AbsoluteUri });

            await _imageRepository.Add(imageEntity);
            await _imageRepository.SaveChanges();

            result.Add(_mapper.Map<ReadImageDTO>(imageEntity));
        }
        return result;
    }

    public async Task<List<ReadImageDTO>> GetAllImages()
    {
        var images = await _imageRepository.Get()
            .Include(i => i.ImageCollection)
            .Include(i => i.Tags)
            .ToListAsync();

        return _mapper.Map<List<ReadImageDTO>>(images);
    }

    public async Task<ReadImageDTO> UpdateImage(string imageId, UpdateImageDTO updateImageDto)
    {
        var image = await _imageRepository.Get()
            .Where(i => i.ImageId.Equals(imageId))
            .Include(i => i.Tags)
            .SingleOrDefaultAsync()
            ?? throw new ArgumentNullException($"The entity: {typeof(Image)} could not be found. {nameof(imageId)} cannot be null or empty. {nameof(imageId)}: {imageId}.");

        var existingImageTagIds = image.Tags.Select(t => t.TagId).OrderBy(t => t).ToArray();
        var updatedImageTagIds = updateImageDto.TagIds.OrderBy(t => t).ToArray();
        
        if(!Enumerable.SequenceEqual(existingImageTagIds, updatedImageTagIds))
        {
            image.Tags.Clear();
            
            var imageTagsToAdd = await _tagRepository.Get().Where(t => updatedImageTagIds.Contains(t.TagId)).ToListAsync();
            foreach (var tag in imageTagsToAdd)
            {
                image.Tags.Add(tag);
            }
        }

        _mapper.Map(updateImageDto, image);
        _imageRepository.Update(image);
        await _imageRepository.SaveChanges();

        return _mapper.Map<ReadImageDTO>(image);
    }

    public async Task DeleteImage(string imageId)
    {
        var image = await _imageRepository.Find(imageId);

        var fileName = image.Uri.Split('/').Last();
        BlobClient client = _blobContainerClient.GetBlobClient(fileName);
        //TODO catch exception with exceptionhandler to return 404
        await client.DeleteAsync();

        _imageRepository.Delete(image);
        await _imageRepository.SaveChanges();
    }
}
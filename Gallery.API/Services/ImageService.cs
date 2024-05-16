using AutoMapper;
using Azure.Storage;
using Azure.Storage.Blobs;
using Gallery.API.DTOs;
using Gallery.API.Interfaces;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using SixLabors.ImageSharp;
using static System.Net.Mime.MediaTypeNames;

namespace Gallery.API.Services;

public class ImageService : IImageService
{
    private readonly IRepository<Database.Entities.Image> _imageRepository;
    private readonly IRepository<ImageCollection> _imageCollectionRepository;
    private readonly IRepository<Tag> _tagRepository;
    private readonly IMapper _mapper;
    private readonly string _storageAccountName;
    private readonly string _storageAccountKey;
    private readonly BlobContainerClient _blobContainerClient;

    public ImageService(
        IConfiguration configuration,
        IRepository<Database.Entities.Image> imageRepository,
        IRepository<ImageCollection> imageCollectionRepository,
        IRepository<Tag> tagRepository,
        IMapper mapper)
    {
        _imageRepository = imageRepository;
        _imageCollectionRepository = imageCollectionRepository;
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
            [FileExtensions(Extensions = "jpg, png, webp, jpeg", ErrorMessage = "Fileformat not supported.")]
            IFormFileCollection images)
    {
        var result = new List<ReadImageDTO>();
        string absoluteUri;

        foreach (var image in images)
        {
            Stream imageStream = image.OpenReadStream();

            if (!image.ContentType.Equals("image/webp", StringComparison.OrdinalIgnoreCase))
            {
                using (var memoryStream = new MemoryStream())
                {
                    imageStream.CopyTo(memoryStream);
                    memoryStream.Position = 0;

                    using (var imageSharp = SixLabors.ImageSharp.Image.Load(memoryStream))
                    {
                        var encoder = new SixLabors.ImageSharp.Formats.Webp.WebpEncoder();
                        memoryStream.SetLength(0);
                        imageSharp.Save(memoryStream, encoder);
                        memoryStream.Position = 0;

                        BlobClient client = _blobContainerClient.GetBlobClient($"{Guid.NewGuid()}.webp");
                        absoluteUri = client.Uri.AbsoluteUri;
                        await client.UploadAsync(memoryStream);
                    }
                }
            }
            else
            {
                BlobClient client = _blobContainerClient.GetBlobClient($"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}");
                absoluteUri = client.Uri.AbsoluteUri;
                await client.UploadAsync(imageStream);
            }

            var imageEntity = _mapper.Map<Database.Entities.Image>(new CreateImageDTO { Uri = absoluteUri });
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
            ?? throw new KeyNotFoundException($"The entity: {typeof(Database.Entities.Image)} with {nameof(imageId)}: {imageId} could not be found.");

        // If the image is added to a collection for the first time.
        if (image.ImageCollectionId is null && updateImageDto.ImageCollectionId is not null)
        {
            await SetOrderOfImageInCollection(image, updateImageDto.ImageCollectionId);
        }
        // If the image is moved to a different collection.
        else if (image.ImageCollectionId is not null && updateImageDto.ImageCollectionId is not null && image.ImageCollectionId != updateImageDto.ImageCollectionId)
        {
            await ReorderImagesInCollection(image, image.ImageCollectionId);
            await SetOrderOfImageInCollection(image, updateImageDto.ImageCollectionId);
        }

        var existingImageTagIds = image.Tags.Select(t => t.TagId).OrderBy(t => t).ToArray();
        var updatedImageTagIds = updateImageDto.TagIds.OrderBy(t => t).ToArray();

        if (!Enumerable.SequenceEqual(existingImageTagIds, updatedImageTagIds))
        {
            image.Tags.Clear();

            var imageTagsToAdd = await _tagRepository.Get().Where(t => updatedImageTagIds.Contains(t.TagId)).ToListAsync();
            foreach (var tag in imageTagsToAdd)
            {
                image.Tags.Add(tag);
            }
        }

        _mapper.Map(updateImageDto, image);
        await _imageRepository.SaveChanges();

        return _mapper.Map<ReadImageDTO>(image);
    }

    public async Task DeleteImage(string imageId)
    {
        var image = await _imageRepository.Get()
            .Where(i => i.ImageId.Equals(imageId))
            .Include(i => i.Tags)
            .SingleOrDefaultAsync()
            ?? throw new KeyNotFoundException($"The entity: {typeof(Database.Entities.Image)} with {nameof(imageId)}: {imageId} could not be found.");

        var fileName = image.Uri.Split('/').Last();
        BlobClient client = _blobContainerClient.GetBlobClient(fileName);
        //TODO catch exception with exceptionhandler to return 404
        await client.DeleteAsync();


        if (image.ImageCollectionId is not null)
        {
            await ReorderImagesInCollection(image, image.ImageCollectionId);
        }

        _imageRepository.Delete(image);
        await _imageRepository.SaveChanges();
    }

    private async Task ReorderImagesInCollection(Database.Entities.Image image, string imageCollectionId)
    {
        var imageCollection = await _imageCollectionRepository.Get()
            .Where(c => c.ImageCollectionId.Equals(imageCollectionId))
        .Include(c => c.Images)
        .SingleOrDefaultAsync() ?? throw new KeyNotFoundException($"The entity: {typeof(ImageCollection)} with {nameof(image.ImageCollectionId)}: {image.ImageCollectionId} could not be found.");

        foreach (var img in imageCollection.Images.Where(i => i.OrderInImageCollection > image.OrderInImageCollection))
        {
            img.OrderInImageCollection--;
        }
    }
    private async Task SetOrderOfImageInCollection(Database.Entities.Image image, string imageCollectionId)
    {
        var imageCollection = await _imageCollectionRepository.Get()
            .Where(c => c.ImageCollectionId.Equals(imageCollectionId))
            .Include(c => c.Images)
            .SingleOrDefaultAsync() ?? throw new KeyNotFoundException($"The entity: {typeof(ImageCollection)} with {nameof(image.ImageCollectionId)}: {image.ImageCollectionId} could not be found.");

        image.OrderInImageCollection = imageCollection.Images.Count;
    }
}
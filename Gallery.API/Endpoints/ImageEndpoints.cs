using Gallery.API.DTOs;
using Gallery.API.Interfaces;

namespace Gallery.API.Endpoints;

public static class ImageEndpoints
{
    public static void MapImageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/images").RequireAuthorization();


        group.MapPost("", UploadImages).DisableAntiforgery();

        group.MapGet("", GetAllImages);

        group.MapPut("{imageId}", UpdateImage);

        group.MapDelete("{imageId}", DeleteImage);
        
    }
    public static async Task<IResult> UploadImages(IFormFileCollection images, IImageService imageService)
    {
        return Results.Created("api/images", await imageService.UploadImages(images));
    }
    public static async Task<IResult> GetAllImages(IImageService imageService)
    {
        return Results.Ok(await imageService.GetAllImages());
    }
    public static async Task<IResult> UpdateImage(IImageService imageService, string imageId, UpdateImageDTO updateImageDto)
    {
        return Results.Ok(await imageService.UpdateImage(imageId, updateImageDto));
    }
    public static async Task<IResult> DeleteImage(IImageService imageService, string imageId)
    {
        await imageService.DeleteImage(imageId);
        return Results.NoContent();
    }
}
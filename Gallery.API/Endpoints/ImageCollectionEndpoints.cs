using Gallery.API.DTOs;
using Gallery.API.Interfaces;

namespace Gallery.API.Endpoints;

public static class ImageCollectionEndpoints
{
    public static void MapImageCollectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/imageCollections").RequireAuthorization();


        group.MapPost("", CreateImageCollection);

        group.MapGet("", GetAllImageCollections).AllowAnonymous();

        group.MapGet("{imageCollectionId}", GetImageCollectionById).AllowAnonymous();

        group.MapPut("{imageCollectionId}", UpdateImageCollection);

        group.MapDelete("{imageCollectionId}", DeleteImageCollection);
        
    }
    public static async Task<IResult> CreateImageCollection(CreateImageCollectionDTO createImageCollectionDto, IImageCollectionService imageCollectionService)
    {
        return Results.Created("api/imageCollections", await imageCollectionService.CreateImageCollection(createImageCollectionDto));
    }
    public static async Task<IResult> GetAllImageCollections(IImageCollectionService imageCollectionService)
    {
        return Results.Ok(await imageCollectionService.GetAllImageCollections());
    }
    public static async Task<IResult> GetImageCollectionById(IImageCollectionService imageCollectionService, string imageCollectionId)
    {
        return Results.Ok(await imageCollectionService.GetImageCollectionById(imageCollectionId));
    }
    public static async Task<IResult> UpdateImageCollection(IImageCollectionService imageCollectionService, string imageCollectionId, UpdateImageCollectionDTO updateImageCollectionDto)
    {
        return Results.Ok(await imageCollectionService.UpdateImageCollection(imageCollectionId, updateImageCollectionDto));
    }
    public static async Task<IResult> DeleteImageCollection(IImageCollectionService imageCollectionService, string imageCollectionId)
    {
        await imageCollectionService.DeleteImageCollection(imageCollectionId);
        return Results.NoContent();
    }
}
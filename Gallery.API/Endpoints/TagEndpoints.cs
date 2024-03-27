using Gallery.API.DTOs;
using Gallery.API.Interfaces;

namespace Gallery.API.Endpoints;

public static class TagEndpoints
{
    public static void MapTagEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("api/tags").RequireAuthorization();


        group.MapPost("", CreateTag);

        group.MapGet("", GetAllTags);

        group.MapPut("{tagId}", UpdateTag);

        group.MapDelete("{tagId}", DeleteTag);
        
    }
    public static async Task<IResult> CreateTag(CreateTagDTO createTagDto, ITagService tagService)
    {
        return Results.Created("api/tags", await tagService.CreateTag(createTagDto));
    }
    public static async Task<IResult> GetAllTags(ITagService tagService)
    {
        return Results.Ok(await tagService.GetAllTags());
    }
    public static async Task<IResult> UpdateTag(ITagService tagService, UpdateTagDTO updateTagDto)
    {
        return Results.Ok(await tagService.UpdateTag(updateTagDto));
    }
    public static async Task<IResult> DeleteTag(ITagService tagService, string tagId)
    {
        await tagService.DeleteTag(tagId);
        return Results.NoContent();
    }
}
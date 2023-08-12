using Gallery.Common.DTOs;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Gallery.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImageCollectionsController : ControllerBase
{
	private readonly IGalleryService _db;
	public ImageCollectionsController(IGalleryService db)
	{
		_db = db;
	}

	[HttpGet]
	public async Task<IResult> Get() 
	{
		await _db.GetAsync<ImageCollection, ImageCollectionDTO>();
		var result = await _db.GetAsync<ImageCollection, ImageCollectionDTO>();
		return Results.Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		_db.Include<ImageCollection>();

		var result = await _db.SingleAsync<ImageCollection, ImageCollectionDTO>(e => e.Id.Equals(id));
		foreach (var item in result.Images)
		{
			item.ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, item.ImageName);
		}
		return Results.Ok(result);
	}

	[HttpPost]
	public async Task<IResult> Post([FromForm] ImageCollectionCreateDTO dto)
	{
		try
		{
			var entity = await _db.AddAsync<ImageCollection, ImageCollectionCreateDTO>(dto);
			if (await _db.SaveChangesAsync())
			{
				return Results.Created($"/imagecollections/{entity.Id}", entity);
			}
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't add the Image Collection.\n{ex}.");
			throw;
		}
		return Results.BadRequest($"Couldn't add the Image Collection.");
	}

	[HttpPut("{id}")]
	public async Task<IResult> Put(int id, [FromForm] ImageCollectionEditDTO dto)
	{
		try
		{
			if (!await _db.AnyAsync<ImageCollection>(e => e.Id.Equals(id))) return Results.NotFound();
			_db.Update<ImageCollection, ImageCollectionEditDTO>(id, dto);
			if (await _db.SaveChangesAsync()) return Results.NoContent();
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't update the Image Collection.\n{ex}.");
		}
		return Results.BadRequest($"Couldn't update the Image Collection.");
	}

	[HttpDelete("{id}")]
	public async Task<IResult> Delete(int id)
	{
		try
		{
			if (!await _db.DeleteAsync<ImageCollection>(id)) return Results.NotFound();
			if (await _db.SaveChangesAsync()) return Results.NoContent();
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't delete the Image Collection.\n{ex}.");
		}
		return Results.BadRequest($"Couldn't delete the Image Collection.");
	}
}
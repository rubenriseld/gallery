//using Gallery.Database.Entities;
//using Gallery.Database.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Gallery.API.Controllers;

//[Route("api/[controller]")]
//[ApiController]
//public class ImagesController : ControllerBase
//{
//	private readonly IGalleryService _db;
//	public ImagesController(IGalleryService db)
//	{
//		_db = db;
//	}

//	[HttpGet]
//	public async Task<IResult> Get() =>
//		Results.Ok(await _db.GetAsync<Image>());

//	[HttpGet("{id}")]
//	public async Task<IResult> Get(int id)
//	{
//		try
//		{
//			var result = await _db.SingleAsync<Image>(e => e.Id.Equals(id));
//			if (result is null) return Results.NotFound();
//			return Results.Ok(result);
//		}
//		catch (Exception ex)
//		{
//			Console.Write(ex);
//			return Results.NotFound();
//		}
		
//	}

//	[HttpPost]
//	public async Task<IResult> Post([FromForm] Image image)
//	{
//		try
//		{
//			var entity = await _db.AddAsync<Image>(image);
//			if (await _db.SaveChangesAsync())
//			{
//				var node = typeof(Image).Name.ToLower();
//				return Results.Created($"/{node}s/{entity.Id}", entity);
//			}
//		}
//		catch (Exception ex)
//		{
//			return Results.BadRequest($"Couldn't add the {typeof(Image).Name} entity.\n{ex}.");
//		}

//		return Results.BadRequest($"Couldn't add the {typeof(Image).Name} entity.");
//	}

//	[HttpPut("{id}")]
//	public async Task<IResult> Put(int id, [FromForm] Image image)
//	{
//		try
//		{
//			if(!await _db.AnyAsync<Image>(e => e.Id.Equals(id))) return Results.NotFound();

//			_db.Update<Image>(id, image);
//			if(await _db.SaveChangesAsync()) return Results.NoContent();
//		}
//		catch (Exception ex)
//		{
//			return Results.BadRequest($"Couldn't update the {typeof(Image).Name} entity. \n{ex}");
//		}
//		return Results.BadRequest($"Couldn't update the {typeof(Image).Name} entity.");
//	}
//	[HttpDelete("{id}")]
//	public async Task<IResult> Delete(int id)
//	{
//		try
//		{
//			if(!await _db.DeleteAsync<Image>(id)) return Results.NotFound();
//			if(await _db.SaveChangesAsync() ) return Results.NoContent();
//		}
//		catch (Exception ex)
//		{
//			return Results.BadRequest($"Couldn't delete the {typeof(Image).Name} entity. \n{ex}");
//		}
//		return Results.BadRequest($"Couldn't delete the {typeof(Image).Name} entity.");

//	}
//}


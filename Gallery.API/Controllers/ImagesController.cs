using Gallery.Common.DTOs;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Gallery.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ImagesController : ControllerBase
{
	private readonly IGalleryService _db;
	private readonly IWebHostEnvironment _hostEnvironment;

	public ImagesController(IGalleryService db, IWebHostEnvironment hostEnvironment)
	{
		_db = db;
		_hostEnvironment = hostEnvironment;
	}

	[HttpGet]
	public async Task<IResult> Get()
	{
		var result = await _db.GetAsync<Image, ImageDTO>();
		foreach (var item in result)
		{
			item.ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, item.ImageName);
		}
		return Results.Ok(result);
	}

	[HttpGet("{id}")]
	public async Task<IResult> Get(int id)
	{
		try
		{

			var result = await _db.SingleAsync<Image, ImageDTO>(e => e.Id.Equals(id));
			result.ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, result.ImageName);
			return Results.Ok(result);
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't get the Images.\n{ex}.");
			throw;
		}
		//return Results.BadRequest($"Couldn't get the Images.");

	}

	[HttpPost]
	public async Task<IResult> Post([FromForm] ImageCreateDTO image)
	{
		try
		{

		//Console.Write(image);
		image.ImageName = await SaveImage(image.ImageFile);
		var result = await _db.AddAsync<Image, ImageCreateDTO>(image);
		if (await _db.SaveChangesAsync())
		{
			return Results.Ok(result);
		}

		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't add the Image.\n{ex}.");
			throw;
		}
		return Results.BadRequest($"Couldn't add the Image.");
	}

	[HttpPut("{id}")]
	public async Task<IResult> Put(int id, [FromForm] ImageEditDTO image)
	{
		if (image.ImageFile != null)
		{
			DeleteImage(image.ImageName);
			image.ImageName = await SaveImage(image.ImageFile);
		}
		try
		{
			if (!await _db.AnyAsync<Image>(e => e.Id.Equals(id))) return Results.NotFound();
			_db.Update<Image, ImageEditDTO>(id, image);
			if (await _db.SaveChangesAsync()) return Results.NoContent();
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't update the Image.\n{ex}.");
		}
		return Results.BadRequest($"Couldn't update the Image.");
	}

	[HttpDelete("{id}")]
	public async Task<IResult> Delete(int id)
	{
		var image = await _db.SingleAsync<Image, ImageDTO>(e => e.Id.Equals(id));
		DeleteImage(image.ImageName);
		try
		{
			if (!await _db.DeleteAsync<Image>(id)) return Results.NotFound();
			if (await _db.SaveChangesAsync()) return Results.NoContent();
		}
		catch (Exception ex)
		{
			return Results.BadRequest($"Couldn't delete the Image.\n{ex}.");
		}
		return Results.BadRequest($"Couldn't delete the Image.");
	} 

	[NonAction]
	public async Task<string> SaveImage(IFormFile imageFile)
	{
		string imageName = new String(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
		imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
		var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
		using (var fileStream = new FileStream(imagePath, FileMode.Create))
		{
			await imageFile.CopyToAsync(fileStream);
		}
		return imageName;
	}
	[NonAction]
	public void DeleteImage(string imageName)
	{
		var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
		if(System.IO.File.Exists(imagePath)) { 
			System.IO.File.Delete(imagePath); 
		}
	}
}
using Gallery.API.Extensions;
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

	// GET: api/<CoursesController>
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
	//public async Task<IResult> Get() =>
	//	await _db.HttpGetAsync<Image, ImageDTO>();

	// GET api/<CoursesController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id) =>
		await _db.HttpSingleAsync<Image, ImageDTO>(id);

	// POST api/<CoursesController>
	[HttpPost]
	public async Task<IResult> Post([FromForm] ImageCreateDTO image)
	{
		//Console.Write(image);
		image.ImageName = await SaveImage(image.ImageFile);
		var result = await _db.HttpPostAsync<Image, ImageCreateDTO>(image);
		return Results.Ok(result);
	}
	// PUT api/<CoursesController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put(int id, [FromForm] ImageEditDTO image)
	{
		if (image.ImageFile != null)
		{
			DeleteImage(image.ImageName);
			image.ImageName = await SaveImage(image.ImageFile);
		}
		var result = await _db.HttpPutAsync<Image, ImageEditDTO>(id, image);
		return Results.Ok(result);

	}


	// DELETE api/<CoursesController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete(int id)
	{
		var image = await _db.SingleAsync<Image, ImageDTO>(e => e.Id.Equals(id));
		DeleteImage(image.ImageName);
		var result = await _db.HttpDeleteAsync<Image>(id);
		return Results.Ok(result);
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


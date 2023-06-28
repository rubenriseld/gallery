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
	public async Task<IResult> Get() =>
		Results.Ok(await _db.GetAsync<ImageCollection>());
}

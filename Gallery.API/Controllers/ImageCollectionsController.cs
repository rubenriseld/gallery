using Gallery.API.Extensions;
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
	// GET: api/<CoursesController>
	[HttpGet]
	public async Task<IResult> Get() =>
		await _db.HttpGetAsync<ImageCollection, ImageCollectionDTO>();

	// GET api/<CoursesController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id) =>
		await _db.HttpSingleAsync<ImageCollection, ImageCollectionDTO>(id);

	// POST api/<CoursesController>
	[HttpPost]
	public async Task<IResult> Post([FromBody] ImageCollectionCreateDTO image) =>
		await _db.HttpPostAsync<ImageCollection, ImageCollectionCreateDTO>(image);

	// PUT api/<CoursesController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put(int id, [FromBody] ImageCollectionEditDTO dto) =>
		await _db.HttpPutAsync<ImageCollection, ImageCollectionEditDTO>(id, dto);


	// DELETE api/<CoursesController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete(int id) =>
		await _db.HttpDeleteAsync<ImageCollection>(id);
}

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
	public ImagesController(IGalleryService db)
	{
		_db = db;
	}


	// GET: api/<CoursesController>
	[HttpGet]
	public async Task<IResult> Get() =>
		await _db.HttpGetAsync<Image, ImageDTO>();

	// GET api/<CoursesController>/5
	[HttpGet("{id}")]
	public async Task<IResult> Get(int id) =>
		await _db.HttpSingleAsync<Image, ImageDTO>(id);

	// POST api/<CoursesController>
	[HttpPost]
	public async Task<IResult> Post([FromBody] ImageCreateDTO image) =>
		await _db.HttpPostAsync<Image, ImageCreateDTO>(image);

	// PUT api/<CoursesController>/5
	[HttpPut("{id}")]
	public async Task<IResult> Put(int id, [FromBody] ImageEditDTO dto) =>
		await _db.HttpPutAsync<Image, ImageEditDTO>(id, dto);


	// DELETE api/<CoursesController>/5
	[HttpDelete("{id}")]
	public async Task<IResult> Delete(int id) =>
		await _db.HttpDeleteAsync<Image>(id);
}


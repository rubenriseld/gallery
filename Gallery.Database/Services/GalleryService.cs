using Gallery.Database.Contexts;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Database.Services;

public class GalleryService : IGalleryService
{
	private readonly GalleryContext _db;

	public GalleryService(GalleryContext db)
	{
		_db = db;
	}

	public async Task<List<TEntity>> GetAsync<TEntity>() where TEntity : class, IEntity
	{
		var entities = await _db.Set<TEntity>().ToListAsync();
		return entities;
	}
}

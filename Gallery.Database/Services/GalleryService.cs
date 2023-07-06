using AutoMapper;
using Gallery.Database.Contexts;
using Gallery.Database.Entities;
using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Update;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Database.Services;

public class GalleryService : IGalleryService
{
	private readonly GalleryContext _db;
	private readonly IMapper _mapper;

	public GalleryService(GalleryContext db, IMapper mapper)
	{
		(_db, _mapper) = (db, mapper);
	}

	public async Task<List<TDto>> GetAsync<TEntity, TDto>()
		where TEntity : class, IEntity
		where TDto : class
	{
		var entities = await _db.Set<TEntity>().ToListAsync();
		return _mapper.Map<List<TDto>>(entities);
	}
	//private SingleAsync for entity
	private async Task<TEntity?> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> expression)
		where TEntity : class, IEntity =>
			await _db.Set<TEntity>().SingleOrDefaultAsync(expression);

	//public SingleAsync for dto
	public async Task<TDto> SingleAsync<TEntity, TDto>(Expression<Func<TEntity, bool>> expression)
		where TEntity : class, IEntity
		where TDto : class
	{
		var entity = await SingleAsync(expression);
		return _mapper.Map<TDto>(entity);
	}

	public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> expression)
		where TEntity : class, IEntity =>
			await _db.Set<TEntity>().AnyAsync(expression);

	public void Include<TEntity>()
		where TEntity : class, IEntity
	{
		var propertyNames = _db.Model.FindEntityType(typeof(TEntity))?
			.GetNavigations().Select(e => e.Name);
		if (propertyNames is null) return;

		foreach (var name in propertyNames)
		{
			_db.Set<TEntity>().Include(name).Load();
		}
	}

	public async Task<bool> SaveChangesAsync() =>
		await _db.SaveChangesAsync() >= 0;

	public async Task<TEntity> AddAsync<TEntity, TDto>(TDto dto)
		where TEntity : class
		where TDto : class
	{
		var entity = _mapper.Map<TEntity>(dto);
		await _db.Set<TEntity>().AddAsync(entity);
		return entity;
	}
	///hej

	public async Task<TEntity> Add<TEntity>(TEntity entity)
		where TEntity : class, IEntity
	{
		await _db.Set<TEntity>().AddAsync(entity);
		return entity;
	}



	public void Update<TEntity, TDto>(int id, TDto dto)
		where TEntity : class, IEntity
		where TDto : class
	{
		var entity = _mapper.Map<TEntity>(dto);
		entity.Id = id;
		_db.Set<TEntity>().Update(entity);
	}



	public async Task<bool> DeleteAsync<TEntity>(int id)
		where TEntity : class, IEntity
	{
		try
		{
			var entity = await SingleAsync<TEntity>(e => e.Id.Equals(id));
			if (entity is null) return false;
			_db.Remove(entity);
		}
		catch
		{
			throw;
		}
		return true;
	}
}

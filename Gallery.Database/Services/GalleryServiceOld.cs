//using Gallery.Database.Contexts;
//using Gallery.Database.Interfaces;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Update;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace Gallery.Database.Services;

//public class GalleryService : IGalleryService
//{
//	private readonly GalleryContext _db;

//	public GalleryService(GalleryContext db)
//	{
//		_db = db;
//	}
	
//	public async Task<List<TEntity>> GetAsync<TEntity>() where TEntity : class, IEntity
//	{
//		var entities = await _db.Set<TEntity>().ToListAsync();
//		return entities;
//	}
//	public async Task<TEntity?> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
//	{
//		var entity = await SingleAsync(expression);
//		return entity;
//	}

//	public async Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> expression) where TEntity : class, IEntity
//		=> await _db.Set<TEntity>().AnyAsync(expression);
//	public async Task<bool> SaveChangesAsync()
//		=> await _db.SaveChangesAsync() >= 0;
	

//	public async Task<TEntity> AddAsync<TEntity>(TEntity entity) where TEntity : class
//	{
//		await _db.Set<TEntity>().AddAsync(entity);
//		return entity;
//	}

//	public void Update<TEntity>(int id, TEntity entity) where TEntity : class, IEntity
//	{
//		entity.Id = id;
//		_db.Set<TEntity>().Update(entity);
//	}
//	public async Task<bool> DeleteAsync<TEntity>(int id) where TEntity : class, IEntity
//	{
//		try
//		{
//			var entity = await SingleAsync<TEntity>(e => e.Id.Equals(id));
//			if (entity is null) return false;
//			_db.Remove(entity);
//		}
//		catch
//		{
//			throw;
//		}
//		return true;
//	}
//}

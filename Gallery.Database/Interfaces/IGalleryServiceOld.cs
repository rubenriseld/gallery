//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Text;
//using System.Threading.Tasks;

//namespace Gallery.Database.Interfaces;

//public interface IGalleryService
//{
//	Task<List<TEntity>> GetAsync<TEntity>() where TEntity : class, IEntity;

//	Task<TEntity> SingleAsync<TEntity>(Expression<Func<TEntity, bool>> expression)
//		where TEntity : class, IEntity;

//	Task<bool> AnyAsync<TEntity>(Expression<Func<TEntity, bool>> expression)
//		where TEntity : class, IEntity;

//	Task<bool> SaveChangesAsync();

//	Task<TEntity> AddAsync<TEntity>(TEntity entity)
//		where TEntity : class;

//	void Update<TEntity>(int id, TEntity entity)
//		where TEntity : class, IEntity;

//	Task<bool> DeleteAsync<TEntity>(int id)
//		where TEntity : class, IEntity;

//}

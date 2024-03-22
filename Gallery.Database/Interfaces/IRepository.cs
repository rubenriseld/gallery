namespace Gallery.Database.Interfaces;

public interface IRepository<TEntity> where TEntity : class
{
    Task Add(TEntity entity);
    void Delete(TEntity entity);
    Task<TEntity> Find(params object[] keyValues);
    IQueryable<TEntity> Get();
    Task<int> SaveChanges();
    void Update(TEntity entity);
}
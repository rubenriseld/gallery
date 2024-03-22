using Gallery.Database.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Gallery.Database;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly GalleryDbContext _dbContext;
    private DbSet<TEntity> _dbSet;
    public Repository(GalleryDbContext context)
    {
        _dbContext = context;
        _dbSet = context.Set<TEntity>();
    }
    public IQueryable<TEntity> Get()
    {
        return _dbSet.AsQueryable();
    }
    public async Task<TEntity> Find(params object[] keyValues)
    {
        if (keyValues is null || keyValues.Length is 0)
        {
            throw new ArgumentNullException($"The entity: {typeof(TEntity)} could not be found. {nameof(keyValues)} cannot be null or empty. {nameof(keyValues)}: {ConvertKeyValuesToString(keyValues)}.");
        }
        var entity = await _dbSet.FindAsync(keyValues);

        return entity is null
            ? throw new KeyNotFoundException($"The entity: {typeof(TEntity)} with {nameof(keyValues)}: {ConvertKeyValuesToString(keyValues)} could not be found.")
            : entity;
    }
    public async Task Add(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException($"The entity: {typeof(TEntity)} {nameof(entity)}: {entity} could not be added. {typeof(TEntity)} cannot be null.");
        }
        await _dbSet.AddAsync(entity);
    }
    public void Delete(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException($"The entity: {typeof(TEntity)} {nameof(entity)}: {entity} could not be deleted. {typeof(TEntity)} cannot be null.");
        }
        _dbSet.Remove(entity);
    }

    public void Update(TEntity entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException($"The entity: {typeof(TEntity)} {nameof(entity)}: {entity} could not be updated. {typeof(TEntity)} cannot be null.");
        }
        _dbSet.Update(entity);
    }

    public async Task<int> SaveChanges() => await _dbContext.SaveChangesAsync();

    private string ConvertKeyValuesToString(params object[] keyValues)
    {
        return string.Join("", keyValues.Select(p => p.ToString()));
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Database.Interfaces;

public interface IGalleryService
{
	Task<List<TEntity>> GetAsync<TEntity>() where TEntity : class, IEntity;
}

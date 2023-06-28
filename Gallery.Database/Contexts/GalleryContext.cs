using Gallery.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Gallery.Database.Contexts
{
	public class GalleryContext : DbContext
	{
		public DbSet<Image> Images => Set<Image>();
		public DbSet<ImageCollection> ImageCollections => Set<ImageCollection>();


		public GalleryContext(DbContextOptions<GalleryContext> options) : base(options)
		{
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			//SeedData(builder);
		}
	}
}

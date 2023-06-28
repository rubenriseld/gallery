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
			SeedData(builder);
		}
		private void SeedData(ModelBuilder builder)
		{
			var imagecollections = new List<ImageCollection>
			{
				new ImageCollection
				{
					Id = 1,
					Name = "collection 1"
				}
			};
			builder.Entity<ImageCollection>().HasData(imagecollections);
			var images = new List<Image>
			{
				new Image
				{
					Id = 1,
					Title = "image 1",
					Description = "desc 1",
					ImageCollectionId = 1,
					ImageName = "test.png"
				}
			};
			builder.Entity<Image>().HasData(images);
		}
	}
}

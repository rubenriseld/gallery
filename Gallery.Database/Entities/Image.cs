using Gallery.Database.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Gallery.Database.Entities
{
	public class Image : IEntity
	{
		public int Id { get; set; }
		[MaxLength(80), Required]
		public string? Title { get; set; }
		[MaxLength(80), Required]
		public string? Description { get; set; }
		public int ImageCollectionId { get; set; }
		public virtual ImageCollection? ImageCollection { get; set; }

		[MaxLength(100)]
		public string? ImageName { get; set; }
		[NotMapped]
		public IFormFile? ImageFile { get; set; }
		[NotMapped]
		public string? ImageSrc { get; set; }
	}
}
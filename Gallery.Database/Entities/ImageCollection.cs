using Gallery.Database.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Database.Entities
{
	public class ImageCollection : IEntity
	{
		public int Id { get; set; }
		[MaxLength(80), Required]
		public string? Name { get; set; }
		public virtual ICollection<Image>? Images { get; set; }
	}
}
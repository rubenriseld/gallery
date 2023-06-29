using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Common.DTOs;

public class ImageBaseDTO
{
	public int Id { get; set; }
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public string ImageName { get; set; } = null!;

}
public class ImageDTO : ImageBaseDTO
{
	public int ImageCollectionId { get; set; }
	public string ImageCollectionName { get; set; }	= null!;
}

public class ImageCreateDTO
{
	public string Title { get; set; } = null!;
	public string Description { get; set; } = null!;
	public int ImageCollectionId { get; set; }
	public string ImageName { get; set; } = null!;
}
public class ImageEditDTO : ImageCreateDTO
{
	public int Id { get; set;}
}

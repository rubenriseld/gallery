using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gallery.Common.DTOs;

public class ImageCollectionDTO
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public List<ImageBaseDTO>? Images { get; set; }
}
public class ImageCollectionCreateDTO
{
	public string Name { get; set; } = null!;
}
public class ImageCollectionEditDTO : ImageCollectionCreateDTO
{
	public int Id { get; set; }	
}

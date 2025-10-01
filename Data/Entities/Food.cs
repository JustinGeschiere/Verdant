using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Food
{
	// Entity properties
	[Key]
	public Guid Id { get; set; }

	[StringLength(256)]
	public string Name { get; set; }

	public decimal Price { get; set; }

	public DateTime Created { get; set; }

	public DateTime Modified { get; set; }

	// Relation properties
	public ICollection<Menu> Menus { get; set; }
}

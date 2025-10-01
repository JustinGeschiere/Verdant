using System.ComponentModel.DataAnnotations;

namespace Data.Entities;

public class Menu
{
	// Entity properties
	[Key]
	public Guid Id { get; set; }

	[StringLength(256)]
	public string Name { get; set; }

	public DateTime Created { get; set; }

	public DateTime Modified { get; set; }

	// Relation properties
	public ICollection<Food> Foods { get; set; }
	public ICollection<Drink> Drinks { get; set; }
}

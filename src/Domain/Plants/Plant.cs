using Domain.Plants.ValueObjects;
using Domain.UserPlants;
using System.ComponentModel.DataAnnotations;

namespace Domain.Plants
{
	public class Plant
	{
		[Key]
		public Guid Id { get; private set; }

		public PlantName Name { get; private set; } = null!;

		private readonly List<UserPlant> _userPlants = new();
		public IReadOnlyCollection<UserPlant> UserPlants => _userPlants;

		private Plant()
		{ }

		public Plant(PlantName name)
		{
			Id = Guid.NewGuid();
			Name = name;
		}
	}
}

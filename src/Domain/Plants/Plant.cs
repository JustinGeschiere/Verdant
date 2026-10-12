using Domain.Plants.ValueObjects;
using Domain.UserPlants;

namespace Domain.Plants;

public class Plant
{
	public Guid Id { get; private set; }

	public PlantName Name { get; private set; } = null!;

	private readonly List<UserPlant> _userPlants = [];
	public IReadOnlyCollection<UserPlant> UserPlants => _userPlants.AsReadOnly();

	private Plant()
	{ }

	public static Plant Create(string givenName, string scientificName)
	{
		var name = PlantName.Create(givenName, scientificName);

		return new Plant()
		{
			Name = name
		};
	}
}

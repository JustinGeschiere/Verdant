using Domain.Shared;

namespace Domain.Plants.ValueObjects;

public sealed class PlantName : ValueObject
{
	public string GivenName { get; private init; } = null!;

	public string ScientificName { get; private init; } = null!;

	private PlantName() 
	{ }

	public static PlantName Create(string givenName, string scientificName)
	{
		if (string.IsNullOrWhiteSpace(givenName))
		{
			throw new ArgumentException("Given name required", nameof(givenName));
		}

		if (string.IsNullOrWhiteSpace(scientificName))
		{
			throw new ArgumentException("Scientific name required", nameof(scientificName));
		}

		return new PlantName()
		{
			GivenName = givenName.Trim(),
			ScientificName = scientificName.Trim()
		};
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return GivenName;
		yield return ScientificName;
	}
}

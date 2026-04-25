using Domain.Shared;

namespace Domain.Plants.ValueObjects
{
	public sealed class PlantName : ValueObject
	{
		public string GivenName { get; }

		public string ScientificName { get; }

		public PlantName(string givenName, string scientificName)
		{
			if (string.IsNullOrWhiteSpace(givenName))
			{
				throw new ArgumentException("Given name required");
			}

			if (string.IsNullOrWhiteSpace(scientificName))
			{
				throw new ArgumentException("Scientific name required");
			}

			GivenName = givenName.Trim();
			ScientificName = scientificName.Trim();
		}

		protected override IEnumerable<object> GetEqualityComponents()
		{
			yield return GivenName;
			yield return ScientificName;
		}
	}
}

using Domain.Shared;

namespace Domain.Users.ValueObjects;

public sealed class PersonName : ValueObject
{
	public string FirstName { get; private init; } = null!;

	public string LastName { get; private init; } = null!;

	public string FullName => $"{FirstName} {LastName}".Trim();

	private PersonName()
	{ }

	public static PersonName Create(string firstName, string lastName)
	{
		if (string.IsNullOrWhiteSpace(firstName))
		{
			throw new ArgumentException("First name required", nameof(firstName));
		}

		if (string.IsNullOrWhiteSpace(lastName))
		{
			throw new ArgumentException("Last name required", nameof(lastName));
		}

		return new PersonName()
		{
			FirstName = firstName,
			LastName = lastName
		};
	}

	protected override IEnumerable<object> GetEqualityComponents()
	{
		yield return FirstName;
		yield return LastName;
	}
}

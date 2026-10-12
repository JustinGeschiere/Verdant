using Domain.UserPlants;
using Domain.Users.ValueObjects;
using Microsoft.AspNetCore.Identity;

namespace Domain.Users;

public class User : IdentityUser<Guid>
{
	public PersonName? Name { get; private set; } = null!;

	private List<UserPlant> _userPlants = [];
	public IReadOnlyCollection<UserPlant> UserPlants => _userPlants.AsReadOnly();

	private User()
	{ }

	public static User Create(string email)
	{
		var user = new User()
		{
			Email = email,
			UserName = email,
		};

		return user;
	}

	public void Register(string firstName, string lastName)
	{
		var name = PersonName.Create(firstName, lastName);
		Name = name;

		if (!EmailConfirmed)
		{
			EmailConfirmed = true;
		}
	}
}

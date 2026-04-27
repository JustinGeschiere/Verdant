using Domain.UserPlants;
using Microsoft.AspNetCore.Identity;

namespace Domain.Users;

public class User : IdentityUser<Guid>
{
	public string? FirstName { get; private set; }

	public string? LastName { get; private set; }


	private List<UserPlant> _userPlants = new();
	public IReadOnlyCollection<UserPlant> UserPlants => _userPlants;

	public string FullName => $"{FirstName} {LastName}".Trim();

	private User()
	{ }

	public User(string email)
	{
		UserName = email;
		Email = email;
	}

	public User(string? firstName, string? lastName)
	{
		FirstName = firstName;
		LastName = lastName;
	}

	public void Activate(string? firstName, string? lastName)
	{
		FirstName = firstName;
		LastName = lastName;

		if (!EmailConfirmed)
		{
			EmailConfirmed = true;
		}
	}
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities;

public class User : IdentityUser<Guid>
{
	[StringLength(128)]
	public string? FirstName { get; set; }

	[StringLength(128)]
	public string? LastName { get; set; }

	[NotMapped]
	public string FullName => $"{FirstName} {LastName}".Trim();
}

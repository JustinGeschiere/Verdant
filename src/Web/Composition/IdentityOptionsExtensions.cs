using Microsoft.AspNetCore.Identity;

namespace Web.Composition
{
	public static class IdentityOptionsExtensions
	{
		public static IdentityOptions UseVerdantPasswordRequirements(this IdentityOptions options)
		{
			options.Password.RequiredLength = 8;
			options.Password.RequireNonAlphanumeric = true;
			options.Password.RequireUppercase = true;
			options.Password.RequireLowercase = true;
			options.Password.RequireDigit = true;

			return options;
		}
	}
}

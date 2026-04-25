using System.ComponentModel.DataAnnotations;

namespace Domain.Users
{
	public class UserRegistration
	{
		public Guid Id { get; set; }

		[Required(AllowEmptyStrings = false)]
		public required string Token { get; set; }

		public DateTime ExpirationDate { get; set; }
	}
}

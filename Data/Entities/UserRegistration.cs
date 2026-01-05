using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
	public class UserRegistration
	{
		public Guid Id { get; set; }

		[Required()]
		public required string Token { get; set; }

		public DateTime ExpirationDate { get; set; }
	}
}

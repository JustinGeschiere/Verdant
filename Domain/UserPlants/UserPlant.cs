using Domain.Plants;
using Domain.Users;

namespace Domain.UserPlants
{
	public class UserPlant
	{
		public Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public Guid PlantId { get; set; }
		public Plant Plant { get; set; } = null!;
	}
}

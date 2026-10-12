using Domain.Plants;
using Domain.Users;

namespace Domain.UserPlants
{
	public class UserPlant
	{
		public Guid UserId { get; private set; }
		public User User { get; private set; } = null!;

		public Guid PlantId { get; private set; }
		public Plant Plant { get; private set; } = null!;

		private UserPlant()
		{ }

		public static UserPlant Create(Guid userId, Guid plantId)
		{
			return new UserPlant()
			{
				UserId = userId,
				PlantId = plantId
			};
		}
	}
}

namespace Data.Entities
{
	public class UserPlant
	{
		public Guid UserId { get; set; }
		public User User { get; set; } = null!;

		public Guid PlantId { get; set; }
		public Plant Plant { get; set; } = null!;
	}
}

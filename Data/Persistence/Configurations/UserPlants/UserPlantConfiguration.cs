using Domain.UserPlants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.UserPlants
{
	public class UserPlantConfiguration : IEntityTypeConfiguration<UserPlant>
	{
		public void Configure(EntityTypeBuilder<UserPlant> builder)
		{
			builder.ToTable("UserPlants");

			builder.HasKey(i => new { i.UserId, i.PlantId });

			// Relationships
			builder.HasOne(i => i.User)
				.WithMany(i => i.UserPlants)
				.HasForeignKey(i => i.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(i => i.Plant)
				.WithMany(i => i.UserPlants)
				.HasForeignKey(i => i.PlantId)
				.OnDelete(DeleteBehavior.Cascade);
		}
	}
}

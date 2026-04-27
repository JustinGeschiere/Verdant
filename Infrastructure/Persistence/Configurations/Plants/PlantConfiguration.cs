using Domain.Plants;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Plants
{
	public class PlantConfiguration : IEntityTypeConfiguration<Plant>
	{
		public void Configure(EntityTypeBuilder<Plant> builder)
		{
			builder.ToTable("Plants");

			builder.HasKey(i  => i.Id);

			// Domain fields
			builder.OwnsOne(i => i.Name, i =>
			{
				i.Property(i => i.GivenName)
					.HasMaxLength(256)
					.IsRequired();

				i.Property(i => i.ScientificName)
					.HasMaxLength(256)
					.IsRequired();
			});

			// Relations
			builder.HasMany(i => i.UserPlants)
				.WithOne(i => i.Plant)
				.HasForeignKey(i => i.PlantId);
		}
	}
}

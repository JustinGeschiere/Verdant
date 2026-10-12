using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Users;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
	public void Configure(EntityTypeBuilder<User> builder)
	{
		// Identity already defines table as AspNetUsers
		builder.ToTable("AspNetUsers");

		// Domain fields
		builder.OwnsOne(i => i.Name, i =>
		{
			i.Property(i => i.FirstName)
				.HasColumnName("FirstName")
				.HasMaxLength(128)
				.IsRequired();

			i.Property(i => i.LastName)
				.HasColumnName("LastName")
				.HasMaxLength(128)
				.IsRequired();

			// Computed
			i.Ignore(i => i.FullName);
		});

		// Allow the Name navigation property to be null
		builder.Navigation(i => i.Name)
			.IsRequired(false);

		// Relationships
		builder.HasMany(i => i.UserPlants)
			.WithOne(i => i.User)
			.HasForeignKey(i => i.UserId);
	}
}

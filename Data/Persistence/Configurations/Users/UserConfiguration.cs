using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations.Users
{
	public class UserConfiguration : IEntityTypeConfiguration<User>
	{
		public void Configure(EntityTypeBuilder<User> builder)
		{
			// Identity already defines table as AspNetUsers
			builder.ToTable("AspNetUsers");

			// Domain fields
			builder.Property(i => i.FirstName)
				.HasMaxLength(128);

			builder.Property(i => i.LastName)
				.HasMaxLength(128);

			// Relationships
			builder.HasMany(i => i.UserPlants)
				.WithOne(i => i.User)
				.HasForeignKey(i => i.UserId);

			// Computed
			builder.Ignore(i => i.FullName);
		}
	}
}

using Domain.Plants;
using Domain.Roles;
using Domain.UserPlant;
using Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VerdantContext(DbContextOptions<VerdantContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
	public DbSet<Plant> Plants { get; set; }

	public DbSet<UserPlant> UserPlants { get; set; }

	public DbSet<UserRegistration> UserRegistrations { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		// Multiple plants <-> multiple users
		modelBuilder.Entity<UserPlant>(e =>
		{
			e.HasKey(i => new { i.PlantId, i.UserId });

			e.HasOne(i => i.Plant)
				.WithMany(i => i.UserPlants)
				.HasForeignKey(i => i.PlantId);

			e.HasOne(i => i.User)
				.WithMany(i => i.UserPlants)
				.HasForeignKey(i => i.UserId);
		});

		// Conversion to widely support TimeSpan properties (Timespan <-> Ticks)
		modelBuilder.Entity<Plant>()
			.Property(i => i.SummerWateringInterval)
			.HasConversion(
				i => i.Ticks,
				i => TimeSpan.FromTicks(i)
			);

		// Conversion to widely support TimeSpan properties (Timespan <-> Ticks)
		modelBuilder.Entity<Plant>()
			.Property(i => i.WinterWateringInterval)
			.HasConversion(
				i => i.Ticks,
				i => TimeSpan.FromTicks(i)
			);
	}
}

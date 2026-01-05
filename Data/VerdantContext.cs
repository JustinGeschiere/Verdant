using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class VerdantContext(DbContextOptions<VerdantContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
	public DbSet<Food> Foods { get; set; }
	public DbSet<Drink> Drinks { get; set; }
	public DbSet<Menu> Menus { get; set; }

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		// Multiple snacks <-> multiple menus relationship
		modelBuilder.Entity<Menu>()
			.HasMany(i => i.Foods)
			.WithMany(i => i.Menus);

		// Multiple drinks <-> multiple menus relationship
		modelBuilder.Entity<Menu>()
			.HasMany(i => i.Drinks)
			.WithMany(i => i.Menus);

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

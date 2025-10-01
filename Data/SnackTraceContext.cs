using Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Data;

public class SnackTraceContext(DbContextOptions<SnackTraceContext> options) : IdentityDbContext<User, Role, Guid>(options)
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
	}
}

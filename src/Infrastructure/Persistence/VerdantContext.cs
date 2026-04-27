using Domain.Plants;
using Domain.Roles;
using Domain.UserPlants;
using Domain.Users;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class VerdantContext(DbContextOptions<VerdantContext> options) : IdentityDbContext<User, Role, Guid>(options)
{
	public DbSet<Plant> Plants { get; set; }

	public DbSet<UserPlant> UserPlants { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.ApplyConfigurationsFromAssembly(typeof(VerdantContext).Assembly);
	}
}

using Application.Composition;
using Domain.Roles;
using Domain.Users;
using Email.Composition;
using Infrastructure.Composition;
using Infrastructure.Composition.Options;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web.Composition;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		// Application registrations
		services.AddApplicationServices();

		// Infrastructure registrations
		services.AddInfrastructureServices(configuration);

		// Presentation registrations
		services.AddEmailServices();

		services.AddHttpContextAccessor();

		services.AddDataProtection()
			.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "data-protection-keys")))
			.SetApplicationName("Verdant");

		// Database
		var sqlOptions = configuration.GetSection(SqlOptions.SECTION)
			.Get<SqlOptions>() ?? throw new InvalidOperationException($"Section {SqlOptions.SECTION} with type '{nameof(SqlOptions)}' not configured.");

		services.AddDbContext<VerdantContext>(options => options.UseNpgsql(sqlOptions.ConnectionString));

		// Identity
		services.AddIdentity<User, Role>(options =>
			{
				options.User.RequireUniqueEmail = true;
				options.SignIn.RequireConfirmedAccount = true;
				options.UseVerdantPasswordRequirements();
			})
			.AddEntityFrameworkStores<VerdantContext>()
			.AddDefaultTokenProviders();

		services.AddControllersWithViews();

		// Setup functionalities
		services.AddSetUpServices();
	}

	public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
	{
		app.UseExceptionHandler("/Error");

		app.UseHttpsRedirection();
		app.UseStaticFiles();

		app.UseRouting();

		app.UseAuthorization();

		app.UseEndpoints(endpoints =>
		{
			endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
			endpoints.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
		});
	}
}

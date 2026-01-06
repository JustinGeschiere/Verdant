using Core.Extensions;
using Core.Options;
using Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Web;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddVerdantOptions();

		// Database
		var sqlOptions = configuration.GetSection(SqlOptions.SECTION)
			.Get<SqlOptions>() ?? throw new InvalidOperationException($"Section {SqlOptions.SECTION} with type {nameof(SqlOptions)} not configured");

		services.AddDbContext<VerdantContext>(options => options.UseNpgsql(sqlOptions.ConnectionString));

		// Identity
		services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddEntityFrameworkStores<VerdantContext>();

		services.AddControllersWithViews();
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

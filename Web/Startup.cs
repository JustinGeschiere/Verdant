using Data;
using Microsoft.AspNetCore.Identity;

namespace Web;

public class Startup(IConfiguration configuration)
{
	public void ConfigureServices(IServiceCollection services)
	{
		services.AddDbContext<SnackTraceContext>(options => options.UseSqlServer(connectionString));

		services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
			.AddEntityFrameworkStores<SnackTraceContext>();


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

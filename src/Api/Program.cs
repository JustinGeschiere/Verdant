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

var builder = WebApplication.CreateBuilder(args);

// Application registrations
builder.Services.AddApplicationServices();

// Infrastructure registrations
builder.Services.AddInfrastructureServices(builder.Configuration);

// Presentation registrations
builder.Services.AddEmailServices();

builder.Services.AddOpenApi();
builder.Services.AddHttpContextAccessor();

builder.Services.AddDataProtection()
	.PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(AppContext.BaseDirectory, "data-protection-keys")))
	.SetApplicationName("Verdant");

// Database
var sqlOptions = builder.Configuration.GetSection(SqlOptions.SECTION)
	.Get<SqlOptions>() ?? throw new InvalidOperationException($"Section {SqlOptions.SECTION} with type '{nameof(SqlOptions)}' not configured.");

builder.Services.AddDbContext<VerdantContext>(options => options.UseNpgsql(sqlOptions.ConnectionString));

// Identity
builder.Services.AddIdentity<User, Role>(options =>
	{
		options.User.RequireUniqueEmail = true;
		options.SignIn.RequireConfirmedAccount = true;
		options.UseVerdantPasswordRequirements();
	})
	.AddEntityFrameworkStores<VerdantContext>()
	.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Setup functionalities
builder.Services.AddSetUpServices();

var app = builder.Build();

app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute("areas", "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.Run();

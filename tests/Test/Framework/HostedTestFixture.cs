using Infrastructure.Persistence;
using IntegrationTests.Framework.Extensions;
using IntegrationTests.Framework.Services.Abstractions;
using IntegrationTests.Framework.Services.Implementations;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Composition;

namespace IntegrationTests.Framework
{
	public abstract class HostedTestFixture<TContext> : HostedTestFixture
		where TContext : DbContext
	{
		private SqliteConnection sqliteConnection = default!;

		public override async Task OneTimeSetUp()
		{
			sqliteConnection = new SqliteConnection("DataSource=:memory:");
			await sqliteConnection.OpenAsync();

			await base.OneTimeSetUp();
		}

		public override async Task OneTimeTearDown()
		{
			await base.OneTimeTearDown();

			sqliteConnection?.Close();
			sqliteConnection?.Dispose();
		}

		public override async Task SetUp()
		{
			await base.SetUp();

			await ScopeAsync(async services =>
			{
				var context = services.GetRequiredService<TContext>();

				await context.Database.EnsureDeletedAsync();
				await context.Database.EnsureCreatedAsync();
			});
		}

		/// <summary>
		/// Override method for implementations to specify custom service registrations
		/// </summary>
		/// <param name="services"></param>
		protected override void ConfigureOverrideServices(IServiceCollection services)
		{
			base.ConfigureOverrideServices(services);

			// Remove any existing database provider registration
			services.RemoveServiceRegistrations<IDbContextOptionsConfiguration<VerdantContext>>();

			// Register Sqlite database provider instead
			services.AddDbContext<VerdantContext>(options =>
			{
				options.UseSqlite(sqliteConnection);
				options.ConfigureWarnings(i => i.Ignore(RelationalEventId.PendingModelChangesWarning));
			});
		}
	}

	[Parallelizable(ParallelScope.None)]
	public abstract class HostedTestFixture
	{
		protected IHost Host { get; private set; } = default!;
		protected TestServer Server { get; private set; } = default!;
		protected IServiceProvider Services { get; private set; } = default!;

		[OneTimeSetUp]
		public virtual async Task OneTimeSetUp()
		{
			Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseEnvironment("Test");
					webBuilder.UseStartup<Startup>();
					webBuilder.UseTestServer();

					// Service configuration overrides
					webBuilder.ConfigureServices(services => ConfigureOverrideServices(services));
				}).Build();

			await Host.StartAsync();

			Server = Host.GetTestServer();
			Services = Host.Services;
		}

		[OneTimeTearDown]
		public virtual async Task OneTimeTearDown()
		{
			if (Host != null)
			{
				await Host.StopAsync();
			}

			Server?.Dispose();
			Host?.Dispose();
		}

		[SetUp]
		public virtual async Task SetUp()
		{
			await ScopeAsync(async services =>
			{
				// Start every test with a clear (anonymous) http context
				var httpContextBuilder = services.GetRequiredService<IHttpContextBuilder>();
				httpContextBuilder.Clear();
			});
		}

		[TearDown]
		public virtual async Task TearDown()
		{
			// No implementation, but still present to provide overridable TearDown to pair with SetUp
		}

		/// <summary>
		/// Override method for implementations to specify custom service registrations
		/// </summary>
		/// <param name="services"></param>
		protected virtual void ConfigureOverrideServices(IServiceCollection services)
		{
			services.AddSingleton<IHttpContextBuilder, HttpContextBuilder>();
		}

		public async Task ScopeAsync(Func<IServiceProvider, Task> action)
		{
			using var scope = Services.CreateScope();

			// Apply http context across async roots
			scope.ServiceProvider.GetRequiredService<IHttpContextBuilder>()
				.Apply();

			await action(scope.ServiceProvider);
		}

		public async Task<T> ScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
		{
			using var scope = Services.CreateScope();

			// Apply http context across async roots
			scope.ServiceProvider.GetRequiredService<IHttpContextBuilder>()
				.Apply();

			return await action(scope.ServiceProvider);
		}

		public Task SendAsync(IRequest request)
		{
			return ScopeAsync(i => i.GetRequiredService<IMediator>().Send(request));
		}

		public Task<T> SendAsync<T>(IRequest<T> request)
		{
			return ScopeAsync(i => i.GetRequiredService<IMediator>().Send(request));
		}
	}
}

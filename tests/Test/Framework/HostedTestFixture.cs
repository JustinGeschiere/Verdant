using Infrastructure.Persistence;
using IntegrationTests.Framework.Extensions;
using IntegrationTests.Framework.Services.Abstractions;
using IntegrationTests.Framework.Services.Implementations;
using Mediator;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;

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
		private sealed class VerdantTestFactory : WebApplicationFactory<Program>
		{
			private readonly Action<IServiceCollection> configureServices;

			public VerdantTestFactory(Action<IServiceCollection> configureServices)
			{
				this.configureServices = configureServices;
			}

			protected override void ConfigureWebHost(IWebHostBuilder builder)
			{
				builder.UseEnvironment("Test");
				builder.ConfigureServices(configureServices);
			}
		}

		protected WebApplicationFactory<Program> Factory { get; private set; } = default!;
		protected IServiceProvider Services { get; private set; } = default!;

		[OneTimeSetUp]
		public virtual async Task OneTimeSetUp()
		{
			Factory = new VerdantTestFactory(ConfigureOverrideServices);
			Services = Factory.Services;

			await Task.CompletedTask;
		}

		[OneTimeTearDown]
		public virtual async Task OneTimeTearDown()
		{
			Factory?.Dispose();

			await Task.CompletedTask;
		}

		[SetUp]
		public virtual async Task SetUp()
		{
			await ScopeAsync(async services =>
			{
				var httpContextBuilder = services.GetRequiredService<IHttpContextBuilder>();
				httpContextBuilder.Clear();
			});
		}

		[TearDown]
		public virtual async Task TearDown()
		{
			await Task.CompletedTask;
		}

		protected virtual void ConfigureOverrideServices(IServiceCollection services)
		{
			services.AddSingleton<IHttpContextBuilder, HttpContextBuilder>();
		}

		public async Task ScopeAsync(Func<IServiceProvider, Task> action)
		{
			using var scope = Services.CreateScope();

			scope.ServiceProvider.GetRequiredService<IHttpContextBuilder>()
				.Apply();

			await action(scope.ServiceProvider);
		}

		public async Task<T> ScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
		{
			using var scope = Services.CreateScope();

			scope.ServiceProvider.GetRequiredService<IHttpContextBuilder>()
				.Apply();

			return await action(scope.ServiceProvider);
		}

		public Task SendAsync(IRequest message)
		{
			return ScopeAsync(i => i.GetRequiredService<IMediator>().Send(message).AsTask());
		}

		public Task<T> SendAsync<T>(IRequest<T> request)
		{
			return ScopeAsync(i => i.GetRequiredService<IMediator>().Send(request).AsTask());
		}
	}
}

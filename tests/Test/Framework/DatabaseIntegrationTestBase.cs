using Infrastructure.Persistence;
using IntegrationTests.Framework.Extensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IntegrationTests.Framework
{
	/// <summary>
	/// Base for database integration tests. 
	/// Manages in-memory SQLite with per-test schema recreation.
	/// </summary>
	[Parallelizable(ParallelScope.None)]
	public abstract class DatabaseIntegrationTestBase<TContext> : IntegrationTestBase
		where TContext : DbContext
	{
		private SqliteConnection? _connection;

		/// <summary>
		/// Called once per test class. 
		/// Creates in-memory SQLite connection and initializes schema.
		/// </summary>
		[OneTimeSetUp]
		public override async Task OneTimeSetUp()
		{
			_connection = new SqliteConnection("DataSource=:memory:");
			await _connection.OpenAsync();

			await base.OneTimeSetUp();
		}

		/// <summary>
		/// Called once per test class after all tests complete. 
		/// Disposes the connection.
		/// </summary>
		[OneTimeTearDown]
		public override async Task OneTimeTearDown()
		{
			await base.OneTimeTearDown();

			_connection?.Close();
			_connection?.Dispose();
		}

		/// <summary>
		/// Recreates database schema before each test for isolation.
		/// </summary>
		[SetUp]
		public virtual async Task SetUp()
		{
			await ScopeAsync(async services =>
			{
				var context = services.GetRequiredService<TContext>();

				// Delete and recreate schema for test isolation
				await context.Database.EnsureDeletedAsync();
				await context.Database.EnsureCreatedAsync();

				// Seed test data if overridden
				await OnDatabaseReady(context);
			});
		}

		/// <summary>
		/// Runs after each test. Override for custom cleanup.
		/// </summary>
		[TearDown]
		public virtual async Task TearDown()
		{
			await Task.CompletedTask;
		}

		/// <summary>
		/// Override to configure database-specific services. 
		/// Replaces PostgreSQL with SQLite.
		/// </summary>
		protected override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			// Remove the production PostgreSQL DbContext registration
			services.RemoveServiceRegistrations<IDbContextOptionsConfiguration<VerdantContext>>();

			// Register in-memory SQLite context instead
			services.AddDbContext<TContext>(options =>
			{
				options.UseSqlite(_connection!);
				options.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
			});
		}

		/// <summary>
		/// Override to seed test data. 
		/// Called after schema is created but before each test.
		/// </summary>
		protected virtual Task OnDatabaseReady(TContext context)
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Get the DbContext directly for assertions or queries.
		/// </summary>
		protected async Task<TContext> GetDbContextAsync()
		{
			return await ScopeAsync(services => Task.FromResult(services.GetRequiredService<TContext>()));
		}
	}
}

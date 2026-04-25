using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Web
{
	public class SetUp : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;

		public SetUp(IServiceProvider serviceProvider, ILogger<SetUp> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			// Database migration
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var context = scope.ServiceProvider.GetRequiredService<VerdantContext>();

				// Check connection resiliently
				var delay = 1000;
				var maxDelay = 60000;
				while (!context.Database.CanConnect())
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					_logger.LogWarning("Failed to connect to database, retrying in {Delay} seconds...", Math.Round(delay/1000f));
					await Task.Delay(delay);

					// Increase delay duration up to max delay duration
					delay = Math.Min(delay * 2, maxDelay);
				}

				// Check pending migration
				var pendingMigrations = context.Database.GetPendingMigrations();

				// Apply pending migrations
				if (pendingMigrations.Count() > 0)
				{
					_logger.LogInformation("Detected {MigrationCount} pending migrations, applying...", pendingMigrations.Count());

					await context.Database.MigrateAsync(cancellationToken);

					_logger.LogInformation("Migrations applied successfully.");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "SetUp failed with exceptions during database migration.");
				throw;
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

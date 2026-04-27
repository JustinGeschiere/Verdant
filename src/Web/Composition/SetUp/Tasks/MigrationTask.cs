using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Web.Composition.SetUpTasks.Abstractions;

namespace Web.Composition.SetUp.Tasks
{
	public class MigrationTask : ISetUpTask
	{
		private readonly VerdantContext _context;

		private readonly ILogger _logger;

		public int Order => 0;

		public MigrationTask(VerdantContext context, ILogger<MigrationTask> logger)
		{
			_context = context;
			_logger = logger;
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			try
			{
				// Check connection resiliently
				var delay = 1000;
				var maxDelay = 60000;
				while (!_context.Database.CanConnect())
				{
					if (cancellationToken.IsCancellationRequested)
					{
						return;
					}

					_logger.LogWarning("Failed to connect to database, retrying in {Delay} seconds...", Math.Round(delay / 1000f));
					await Task.Delay(delay);

					// Increase delay duration up to max delay duration
					delay = Math.Min(delay * 2, maxDelay);
				}

				// Check pending migration
				var pendingMigrations = _context.Database.GetPendingMigrations();

				// Apply pending migrations
				if (pendingMigrations.Count() > 0)
				{
					_logger.LogInformation("Detected {MigrationCount} pending migrations, applying...", pendingMigrations.Count());

					await _context.Database.MigrateAsync(cancellationToken);

					_logger.LogInformation("Migrations applied successfully.");
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "Migration task failed with exceptions.");
				throw;
			}
		}
	}
}

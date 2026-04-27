using Web.Composition.SetUpTasks.Abstractions;

namespace Web.Composition.SetUp
{
	public class SetUpHostedService : IHostedService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly ILogger _logger;

		public SetUpHostedService(IServiceProvider serviceProvider, ILogger<SetUpHostedService> logger)
		{
			_serviceProvider = serviceProvider;
			_logger = logger;
		}

		public async Task StartAsync(CancellationToken cancellationToken)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var tasks = scope.ServiceProvider.GetRequiredService<IEnumerable<ISetUpTask>>();

				foreach (var task in tasks.OrderBy(i => i.Order))
				{
					await task.ExecuteAsync(cancellationToken);
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "SetUp failed with exceptions.");
				throw;
			}
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			return Task.CompletedTask;
		}
	}
}

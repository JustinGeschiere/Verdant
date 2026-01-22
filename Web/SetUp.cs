using Microsoft.EntityFrameworkCore;
using Web.SetUpTasks.Abstractions;

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

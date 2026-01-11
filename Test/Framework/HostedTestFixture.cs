using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web;

namespace Test.Framework
{
	public abstract class HostedTestFixture
	{
		protected IHost Host { get; private set; } = default!;
		protected TestServer Server { get; private set; } = default!;
		protected IServiceProvider Services { get; private set; } = default!;

		[OneTimeSetUp]
		public async Task OneTimeSetup()
		{
			Host = Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder()
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseEnvironment("Test");
					webBuilder.UseStartup<Startup>();
					webBuilder.UseTestServer();
				}).Build();

			// Service configuration overrides
			ConfigureServices();

			await Host.StartAsync();

			Server = Host.GetTestServer();
			Services = Host.Services;
		}

		[OneTimeTearDown]
		public async Task OneTimeTeardown()
		{
			if (Host != null)
			{
				await Host.StopAsync();
			}

			Server?.Dispose();
			Host?.Dispose();
		}

		protected virtual void ConfigureServices()
		{
			// Override method for implementations to specify custom service registrations
		}

		public async Task ScopeAsync(Func<IServiceProvider, Task> action)
		{
			using var scope = Services.CreateScope();
			await action(scope.ServiceProvider);
		}

		public async Task<T> ScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
		{
			using var scope = Services.CreateScope();
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

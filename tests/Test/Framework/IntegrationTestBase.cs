using Mediator;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace IntegrationTests.Framework
{
	/// <summary>
	/// Base for integration tests. Manages WebApplicationFactory lifecycle and provides DI access.
	/// </summary>
	[Parallelizable(ParallelScope.None)]
	public abstract class IntegrationTestBase
	{
		private ServiceConfigurableTestFactory? _factory;
		protected IServiceProvider Services { get; private set; } = null!;

		/// <summary>
		/// Called once per test class. 
		/// Creates and initializes the factory.
		/// </summary>
		[OneTimeSetUp]
		public virtual async Task OneTimeSetUp()
		{
			_factory = new ServiceConfigurableTestFactory(configureServices: ConfigureServices);

			// Trigger host build by creating a client - this ensures full configuration pipeline runs
			using var _ = _factory.CreateClient();

			Services = _factory.Services;

			await OnTestFixtureInitialize();
		}

		/// <summary>
		/// Called once per test class after all tests complete.
		/// </summary>
		[OneTimeTearDown]
		public virtual async Task OneTimeTearDown()
		{
			await OnTestFixtureDispose();
			_factory?.Dispose();
		}

		/// <summary>
		/// Override to configure test-specific services.
		/// </summary>
		protected virtual void ConfigureServices(IServiceCollection services)
		{
		}

		/// <summary>
		/// Override for async setup after fixture initialization.
		/// </summary>
		protected virtual Task OnTestFixtureInitialize()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Override for async cleanup after all tests complete.
		/// </summary>
		protected virtual Task OnTestFixtureDispose()
		{
			return Task.CompletedTask;
		}

		/// <summary>
		/// Execute an async action within a service scope.
		/// </summary>
		protected async Task ScopeAsync(Func<IServiceProvider, Task> action)
		{
			using var scope = Services.CreateScope();
			await action(scope.ServiceProvider);
		}

		/// <summary>
		/// Execute an async function within a service scope and return a result.
		/// </summary>
		protected async Task<T> ScopeAsync<T>(Func<IServiceProvider, Task<T>> action)
		{
			using var scope = Services.CreateScope();
			return await action(scope.ServiceProvider);
		}

		/// <summary>
		/// Send a mediator request.
		/// </summary>
		protected Task SendAsync(IRequest message)
		{
			return ScopeAsync(services => services.GetRequiredService<IMediator>().Send(message).AsTask());
		}

		/// <summary>
		/// Send a mediator request with a return value.
		/// </summary>
		protected Task<T> SendAsync<T>(IRequest<T> request)
		{
			return ScopeAsync(services => services.GetRequiredService<IMediator>().Send(request).AsTask());
		}

		/// <summary>
		/// Resolve a service directly from the DI container.
		/// </summary>
		protected T GetService<T>() where T : notnull
		{
			return Services.GetRequiredService<T>();
		}

		/// <summary>
		/// Resolve a scoped service for a test operation.
		/// </summary>
		protected async Task<T> GetScopedServiceAsync<T>(Func<T, Task> action) where T : notnull
		{
			return await ScopeAsync(async services =>
			{
				var service = services.GetRequiredService<T>();
				await action(service);
				return service;
			});
		}
	}
}

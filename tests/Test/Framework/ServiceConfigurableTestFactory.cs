using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Framework
{
	/// <summary>
	/// Custom WebApplicationFactory for test environment and service overrides.
	/// </summary>
	public sealed class ServiceConfigurableTestFactory : WebApplicationFactory<Program>
	{
		private readonly string _environment;
		private readonly Action<IServiceCollection>? _configureServices;

		public ServiceConfigurableTestFactory(string environment = "Test", Action<IServiceCollection>? configureServices = null)
		{
			_environment = environment;
			_configureServices = configureServices;
		}

		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			builder.UseEnvironment(_environment);

			if (_configureServices != null)
			{
				builder.ConfigureServices(_configureServices);
			}
		}
	}
}

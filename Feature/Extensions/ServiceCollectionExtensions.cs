using Microsoft.Extensions.DependencyInjection;

namespace Feature.Extensions
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddVerdantFeatures(this IServiceCollection services)
		{
			services.AddMediatR(options => options.RegisterServicesFromAssembly(typeof(ServiceCollectionExtensions).Assembly));

			return services;
		}
	}
}

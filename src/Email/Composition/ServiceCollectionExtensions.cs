using Email.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace Email.Composition
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddEmailServices(this IServiceCollection services)
		{
			services.AddScoped<IMailRenderer, BlazorMailRenderer>();

			return services;
		}
	}
}

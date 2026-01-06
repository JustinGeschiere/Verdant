using Core.Options;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Extensions
{
	public static class ServiceCollectionExtentions
	{
		public static IServiceCollection AddVerdantOptions(this IServiceCollection services)
		{
			// TODO: Do we ever inject this outside of startup logic where it is manually bound?
			services.AddOptions<SqlOptions>()
				.BindConfiguration(SqlOptions.SECTION)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			return services;
		}
	}
}

using Microsoft.Extensions.DependencyInjection;

namespace Test.Framework.Helpers
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection RemoveServiceRegistrations<T>(this IServiceCollection services)
		{
			var descriptors = services.Where(i => i.ServiceType == typeof(T));
			foreach (var descriptor in descriptors.ToList())
			{
				services.Remove(descriptor);
			}

			return services;
		}
	}
}

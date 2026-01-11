using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Framework.Helpers
{
	public static class TestHelper
	{
		public static void ReplaceServiceRegistration<T>()
		{

		}

		public static IServiceCollection RemoveServiceRegistration<T>(this IServiceCollection services)
		{
			var descriptor = services.SingleOrDefault(i => i.ServiceType == typeof(T));
			if (descriptor != null)
			{
				services.Remove(descriptor);
			}

			return services;
		}
	}
}

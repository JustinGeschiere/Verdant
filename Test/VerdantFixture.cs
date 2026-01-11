using Microsoft.Extensions.DependencyInjection;
using Test.Framework;

namespace Test
{
	[TestFixture]
	public class VerdantFixture : HostedTestFixture
	{
		protected override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);
		}
	}
}

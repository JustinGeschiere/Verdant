using Data;
using Microsoft.Extensions.DependencyInjection;
using Test.Framework;

namespace Test
{
	[TestFixture]
	public class VerdantFixture : HostedTestFixture<VerdantContext>
	{
		protected override void ConfigureOverrideServices(IServiceCollection services)
		{
			base.ConfigureOverrideServices(services);
		}
	}
}

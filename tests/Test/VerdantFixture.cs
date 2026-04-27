using Infrastructure.Mailing.Abstractions;
using Infrastructure.Persistence;
using IntegrationTests.Framework;
using IntegrationTests.Framework.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTests
{
	[TestFixture]
	public class VerdantFixture : HostedTestFixture<VerdantContext>
	{
		protected override void ConfigureOverrideServices(IServiceCollection services)
		{
			base.ConfigureOverrideServices(services);

			// Override token lifetimes to include expiration behavior in tests
			services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromSeconds(5);
			});

			// Replace mail sender service with mocked instance
			services.RemoveServiceRegistrations<IMailSender>();
			var mailSenderMock = new Mock<IMailSender>();

			mailSenderMock
				.Setup(i => i.SendHtmlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(true);

			services.AddSingleton<IMailSender>(mailSenderMock.Object);
		}
	}
}

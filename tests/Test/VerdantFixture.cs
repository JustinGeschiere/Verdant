using Infrastructure.Mailing.Abstractions;
using Infrastructure.Persistence;
using IntegrationTests.Framework;
using IntegrationTests.Framework.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace IntegrationTests
{
	/// <summary>
	/// Test fixture with database and mocked email service.
	/// </summary>
	public class VerdantFixture : DatabaseIntegrationTestBase<VerdantContext>
	{
		protected override void ConfigureServices(IServiceCollection services)
		{
			base.ConfigureServices(services);

			// Override token lifetimes to test expiration behavior
			services.Configure<DataProtectionTokenProviderOptions>(options =>
			{
				options.TokenLifespan = TimeSpan.FromSeconds(5);
			});

			// Mock the email service
			services.RemoveServiceRegistrations<IMailSender>();

			var mailSenderMock = new Mock<IMailSender>();
			mailSenderMock.Setup(i => i.SendHtmlAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
				.ReturnsAsync(true);

			services.AddSingleton<IMailSender>(mailSenderMock.Object);
		}
	}
}

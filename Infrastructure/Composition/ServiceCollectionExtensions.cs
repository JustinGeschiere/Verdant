using Infrastructure.Mailing;
using Infrastructure.Mailing.Abstractions;
using Infrastructure.Mailing.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Composition
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddMailing(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions<SmtpOptions>()
				.Bind(configuration.GetSection(SmtpOptions.SECTION))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddScoped<IMailSender, MailKitMailSender>();

			return services;
		}
	}
}

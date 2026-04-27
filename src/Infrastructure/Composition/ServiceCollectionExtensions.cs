using Infrastructure.Composition.Options;
using Infrastructure.Mailing;
using Infrastructure.Mailing.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Composition
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddOptions<SqlOptions>()
				.BindConfiguration(SqlOptions.SECTION)
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddOptions<SmtpOptions>()
				.Bind(configuration.GetSection(SmtpOptions.SECTION))
				.ValidateDataAnnotations()
				.ValidateOnStart();

			services.AddScoped<IMailSender, MailKitMailSender>();

			return services;
		}
	}
}

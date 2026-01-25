using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Service.Mailing.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Mailing.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMailingService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<SmtpOptions>()
                .Bind(configuration.GetSection(SmtpOptions.SECTION))
                .ValidateDataAnnotations()
                .ValidateOnStart();

            return services;
        }
    }
}

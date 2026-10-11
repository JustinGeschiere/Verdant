using Api.Composition.SetUp;
using Api.Composition.SetUp.Abstractions;
using Api.Composition.SetUp.Tasks;

namespace Api.Composition
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSetUpServices(this IServiceCollection services)
        {
            // Tasks
            services.AddScoped<ISetUpTask, MigrationTask>();
            services.AddScoped<ISetUpTask, IdentityRolesTask>();
            services.AddScoped<ISetUpTask, SystemUserTask>();

            // Hosted service that runs tasks on startup
            services.AddHostedService<SetUpHostedService>();

            return services;
        }
    }
}

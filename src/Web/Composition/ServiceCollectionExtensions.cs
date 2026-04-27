using Web.Composition.SetUp;
using Web.Composition.SetUp.Tasks;
using Web.Composition.SetUpTasks.Abstractions;

namespace Web.Composition
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddSetUpServices(this IServiceCollection services)
        {
            // Tasks
            services.AddScoped<ISetUpTask, MigrationTask>();
            services.AddScoped<ISetUpTask, IdentityRolesTask>();
            services.AddScoped<ISetUpTask, SystemUserTask>();

            // Hosted service
            services.AddHostedService<SetUpHostedService>();

            return services;
        }
    }
}

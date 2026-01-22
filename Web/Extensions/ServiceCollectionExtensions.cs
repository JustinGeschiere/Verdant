using Web.SetUpTasks;
using Web.SetUpTasks.Abstractions;

namespace Web.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddVerdantSetUp(this IServiceCollection services)
        {
            // Tasks
            services.AddScoped<ISetUpTask, MigrationTask>();
            services.AddScoped<ISetUpTask, IdentityRolesTask>();
            services.AddScoped<ISetUpTask, SystemUserTask>();

            // Hosted service
            services.AddHostedService<SetUp>();

            return services;
        }
    }
}

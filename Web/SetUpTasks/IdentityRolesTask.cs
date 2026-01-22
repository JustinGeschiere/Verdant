using Data.Entities;
using Microsoft.AspNetCore.Identity;
using Web.Identity;
using Web.SetUpTasks.Abstractions;

namespace Web.SetUpTasks
{
    public class IdentityRolesTask : ISetUpTask
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly ILogger _logger;

        public int Order => 10;

        public IdentityRolesTask(RoleManager<Role> roleManager, ILogger<IdentityRolesTask> logger)
        {
            _roleManager = roleManager;
            _logger = logger;
        }

        public async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            try
            {
                foreach (var role in VerdantRoles.All)
                {
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new Role() { Name = role });
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Identity roles task failed with exceptions.");
                throw;
            }
        }
    }
}

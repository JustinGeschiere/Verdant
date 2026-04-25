using Application.Features.Users.InviteUser;
using Domain.Roles;
using Domain.Users;
using Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Identity;
using Web.SetUpTasks.Abstractions;

namespace Web.SetUpTasks
{
	public class SystemUserTask : ISetUpTask
	{
		private readonly VerdantContext _context;
		private readonly IMediator _mediator;
		private readonly UserManager<User> _userManager;
		private readonly RoleManager<Role> _roleManager;
		private readonly ILogger _logger;

		public int Order => 20;

		public SystemUserTask(VerdantContext context, IMediator mediator, UserManager<User> userManager, RoleManager<Role> roleManager, ILogger<SystemUserTask> logger)
		{
			_context = context;
			_mediator = mediator;
			_userManager = userManager;
			_roleManager = roleManager;
			_logger = logger;
		}

		public async Task ExecuteAsync(CancellationToken cancellationToken)
		{
			try
			{
				var systemRole = await _roleManager.FindByNameAsync(VerdantRoles.System);
				var hasSystemUser = await _context.UserRoles.AnyAsync(i => i.RoleId == systemRole!.Id, cancellationToken);

				// We only create a system user if there is no existing system user
				if (!hasSystemUser)
				{
					// TODO: Look at config instead?
					var systemUserEmail = "system@verdant.com";

					// TODO: Do we want to rely on the invite feature here?
					var request = new InviteUserCommand()
					{
						Email = systemUserEmail
					};

					var result = await _mediator.Send(request);

					if (result.IsSuccess)
					{
						var systemUser = await _userManager.FindByIdAsync(result.Value!.UserId.ToString()!);

						var roleResult = await _userManager.AddToRoleAsync(systemUser!, VerdantRoles.System);
						if (roleResult.Succeeded)
						{
							_logger.LogInformation("Invited system user account with user id '{UserID}' and registration token '{Token}'", result.Value!.UserId, result.Value!.Token);
						}
						else
						{
							throw new InvalidOperationException($"System user could not be assigned the role '{VerdantRoles.System}'");
						}
					}
					else
					{
						throw new InvalidOperationException($"System user was not created with status '{result.Error!.Message}'");
					}
				}
			}
			catch (Exception e)
			{
				_logger.LogError(e, "System user task failed with exceptions.");
				throw;
			}
		}
	}
}

using Data.Entities;
using Feature.Framework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Feature.Users
{
	public class InviteUserCommand
	{
		public record Request : IRequest<Result>
		{
			[Required(AllowEmptyStrings = false)]
			public required string Email { get; init; }
		}

		public record Result(ResultStatus status) : BaseResult<ResultStatus>(status)
		{
			public Guid? UserId { get; init; }

			public string? RegisterToken { get; init; }
		}

		public enum ResultStatus
		{
			Success,
			SuccessWithResend,
			AlreadyExists,
			GeneralError
		}

		public class Handler : IRequestHandler<Request, Result>
		{
			private readonly UserManager<User> _userManager;
			private readonly ILogger _logger;

			public Handler(UserManager<User> userManager, ILogger<TemplateCommand> logger)
			{
				_userManager = userManager;
				_logger = logger;
			}

			public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
			{
				var existingUser = await _userManager.FindByEmailAsync(request.Email);
				if (existingUser != null && existingUser.EmailConfirmed)
				{
					_logger.LogWarning("User with e-mail '{Email}' already exists.", request.Email);
					return new Result(ResultStatus.AlreadyExists);
				}
				else if (existingUser != null && !existingUser.EmailConfirmed)
				{
					// HACK: Normally we would send an e-mail, but for this project we just display the registration link
					_logger.LogInformation("Recreating registration for user with e-mail '{Email}'.", request.Email);
					return new Result(ResultStatus.SuccessWithResend)
					{
						UserId = existingUser.Id,
						RegisterToken = await _userManager.GenerateEmailConfirmationTokenAsync(existingUser)
					};
				}

				var user = new User()
				{
					UserName = request.Email,
					Email = request.Email
				};

				var createResult = await _userManager.CreateAsync(user);
				if (!createResult.Succeeded)
				{
					_logger.LogError("Unexpected error while creating user with e-mail '{Email}'.", request.Email);
					return new Result(ResultStatus.GeneralError);
				}

				// HACK: Normally we would send an e-mail, but for this project we just display the registration link
				_logger.LogInformation("Creating registration for user with e-mail '{Email}'.", request.Email);
				return new Result(ResultStatus.Success)
				{
					UserId = user.Id,
					RegisterToken = await _userManager.GenerateEmailConfirmationTokenAsync(user)
				};
			}
		}
	}
}

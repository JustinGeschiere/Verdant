using Application.Common.Errors;
using Application.Common.Results;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.ConfirmResetPassword
{
	public sealed class ConfirmResetPasswordHandler : IRequestHandler<ConfirmResetPasswordCommand, Result>
	{
		private readonly UserManager<User> _userManager;
		private readonly ILogger _logger;

		public ConfirmResetPasswordHandler(UserManager<User> userManager, ILogger<ConfirmResetPasswordHandler> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<Result> Handle(ConfirmResetPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId.ToString() ?? string.Empty);
			if (user == null)
			{
				return Result.Failure(UserErrors.NotFound);
			}

			var passwordResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
			if (!passwordResult.Succeeded)
			{
				// Prioritize the token validity over password validity
				if (passwordResult.Errors.Any(i => i.Code.Equals("InvalidToken")))
				{
					_logger.LogWarning("Something went wrong while consuming password token for user with id '{UserId}'", user.Id);
					return Result.Failure(UserErrors.InvalidToken);
				}

				_logger.LogWarning("Something went wrong while setting password for user with id '{UserId}'", user.Id);
				return Result.Failure(UserErrors.InvalidPassword);
			}

			// TODO: Move this to register user use case
			if (!user.EmailConfirmed)
			{
				user.EmailConfirmed = true;
				var updateResult = await _userManager.UpdateAsync(user);
				if (!updateResult.Succeeded)
				{
					return Result.Failure(CommonErrors.Failure);
				}
			}

			return Result.Success();
		}
	}
}

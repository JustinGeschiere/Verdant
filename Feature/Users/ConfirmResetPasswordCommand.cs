using Data.Entities;
using Feature.Framework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel.DataAnnotations;

namespace Feature.Users
{
	public class ConfirmResetPasswordCommand
	{
		public record Request : IRequest<Result>
		{
			[Required]
			public required Guid? UserId { get; init; }

			[Required(AllowEmptyStrings = false)]
			public required string Token { get; init; }

			[Required(AllowEmptyStrings = false)]
			[DataType(DataType.Password)]
			public required string Password { get; init; }

			[DataType(DataType.Password)]
			[Compare("Password")]
			public required string ConfirmPassword { get; init; }
		}

		public record Result(ResultStatus status) : BaseResult<ResultStatus>(status)
		{ }

		public enum ResultStatus
		{
			Success,
			NotFound,
			InvalidToken,
			InvalidPassword,
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
				var user = await _userManager.FindByIdAsync(request.UserId.ToString() ?? string.Empty);
				if (user == null)
				{
					return new Result(ResultStatus.NotFound);
				}

				var passwordResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
				if (!passwordResult.Succeeded)
				{
					// Prioritize the token validity over password validity
					if (passwordResult.Errors.Any(i => i.Code.Equals("InvalidToken")))
					{
						_logger.LogWarning("Something went wrong while consuming password token for user with id '{UserId}'", user.Id);
						return new Result(ResultStatus.InvalidToken);
					}

					_logger.LogWarning("Something went wrong while setting password for user with id '{UserId}'", user.Id);
					return new Result(ResultStatus.InvalidPassword);
				}

				if (!user.EmailConfirmed)
				{
					user.EmailConfirmed = true;
					var updateResult = await _userManager.UpdateAsync(user);
					if (!updateResult.Succeeded)
					{
						return new Result(ResultStatus.GeneralError);
					}
				}

				return new Result(ResultStatus.Success);
			}
		}
	}
}

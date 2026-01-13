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

			public string? Token { get; init; }
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
			private readonly IMediator _mediator;
			private readonly ILogger _logger;

			public Handler(UserManager<User> userManager, IMediator mediator, ILogger<TemplateCommand> logger)
			{
				_userManager = userManager;
				_mediator = mediator;
				_logger = logger;
			}

			public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
			{
				var existingUser = await _userManager.FindByEmailAsync(request.Email);
				if (existingUser != null)
				{
					var hasPassword = await _userManager.HasPasswordAsync(existingUser);

					if (hasPassword)
					{
						return new Result(ResultStatus.AlreadyExists);
					}
					else
					{
						var resendPasswordTokenRequest = new RequestResetPasswordCommand.Request()
						{ 
							UserId = existingUser.Id 
						};

						var resendResult = await _mediator.Send(resendPasswordTokenRequest);
						if (resendResult.Status == RequestResetPasswordCommand.ResultStatus.Success)
						{
							return new Result(ResultStatus.SuccessWithResend)
							{
								UserId = resendResult.UserId,
								Token = resendResult.Token
							};
						}
						else
						{
							_logger.LogError("Something went wrong while creating password reset token for existing user with e-mail '{Email}'.", request.Email);
							return new Result(ResultStatus.GeneralError);
						}
					}
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

				var passwordTokenRequest = new RequestResetPasswordCommand.Request()
				{
					UserId = user.Id
				};

				var requestResult = await _mediator.Send(passwordTokenRequest);
				if (requestResult.Status == RequestResetPasswordCommand.ResultStatus.Success)
				{
					return new Result(ResultStatus.Success)
					{
						UserId = requestResult.UserId,
						Token = requestResult.Token
					};
				}
				else
				{
					_logger.LogError("Something went wrong while creating password reset token for new user with e-mail '{Email}'.", request.Email);
					return new Result(ResultStatus.GeneralError);
				}
			}
		}
	}
}

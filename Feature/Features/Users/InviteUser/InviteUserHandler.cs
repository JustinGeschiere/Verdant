using Application.Common.Errors;
using Application.Common.Results;
using Application.Features.Users.RequestResetPassword;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.InviteUser
{
	public sealed class InviteUserHandler : IRequestHandler<InviteUserCommand, Result<InviteUserResult>>
	{
		private readonly UserManager<User> _userManager;
		private readonly IMediator _mediator;
		private readonly ILogger _logger;

		public InviteUserHandler(UserManager<User> userManager, IMediator mediator, ILogger<InviteUserCommand> logger)
		{
			_userManager = userManager;
			_mediator = mediator;
			_logger = logger;
		}

		public async Task<Result<InviteUserResult>> Handle(InviteUserCommand request, CancellationToken cancellationToken)
		{
			var existingUser = await _userManager.FindByEmailAsync(request.Email);
			if (existingUser != null)
			{
				var hasPassword = await _userManager.HasPasswordAsync(existingUser);

				if (hasPassword)
				{
					return Result<InviteUserResult>.Failure(UserErrors.NotUnique);
				}
				else
				{
					var resendPasswordTokenRequest = new RequestResetPasswordCommand()
					{
						UserId = existingUser.Id
					};

					var resendResult = await _mediator.Send(resendPasswordTokenRequest);
					if (resendResult.IsSuccess)
					{
						return Result<InviteUserResult>.Success(new InviteUserResult(resendResult.Value!.UserId, resendResult.Value!.Token, resend: true));
					}
					else
					{
						_logger.LogError("Something went wrong while creating password reset token for existing user with e-mail '{Email}'.", request.Email);
						return Result<InviteUserResult>.Failure(CommonErrors.Failure);
					}
				}
			}

			var user = new User(request.Email);

			var createResult = await _userManager.CreateAsync(user);
			if (!createResult.Succeeded)
			{
				_logger.LogError("Unexpected error while creating user with e-mail '{Email}'.", request.Email);
				return Result<InviteUserResult>.Failure(CommonErrors.Failure);
			}

			var passwordTokenRequest = new RequestResetPasswordCommand()
			{
				UserId = user.Id
			};

			var requestResult = await _mediator.Send(passwordTokenRequest);
			if (requestResult.IsSuccess)
			{

				return Result<InviteUserResult>.Success(new InviteUserResult(requestResult.Value!.UserId, requestResult.Value!.Token, resend: false));
			}
			else
			{
				_logger.LogError("Something went wrong while creating password reset token for new user with e-mail '{Email}'.", request.Email);
				return Result<InviteUserResult>.Failure(CommonErrors.Failure);
			}
		}
	}
}

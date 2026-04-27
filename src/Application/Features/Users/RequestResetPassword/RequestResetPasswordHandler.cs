using Application.Common.Errors;
using Application.Common.Results;
using Domain.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Features.Users.RequestResetPassword
{
	public class RequestResetPasswordHandler : IRequestHandler<RequestResetPasswordCommand, Result<RequestResetPasswordResult>>
	{
		private readonly UserManager<User> _userManager;
		private readonly ILogger _logger;

		public RequestResetPasswordHandler(UserManager<User> userManager, ILogger<RequestResetPasswordHandler> logger)
		{
			_userManager = userManager;
			_logger = logger;
		}

		public async Task<Result<RequestResetPasswordResult>> Handle(RequestResetPasswordCommand request, CancellationToken cancellationToken)
		{
			var user = await _userManager.FindByIdAsync(request.UserId.ToString());
			if (user == null)
			{
				_logger.LogWarning("Password reset requested for user with id '{UserId}' that does not exist", request.UserId);
				return Result<RequestResetPasswordResult>.Failure(UserErrors.NotFound);
			}

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);

			return Result<RequestResetPasswordResult>.Success(new RequestResetPasswordResult(user.Id, token));
		}
	}
}

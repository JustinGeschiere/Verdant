using Domain.Users;
using Feature.Framework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Feature.Users
{
	public class RequestResetPasswordCommand
	{
		public record Request : IRequest<Result>
		{
			[Required]
			public required Guid? UserId { get; init; }
		}

		public record Result(ResultStatus status) : BaseResult<ResultStatus>(status)
		{
			public Guid? UserId { get; init; }

			public string? Token { get; init; }
		}

		public enum ResultStatus
		{
			Success,
			NotFound
		}

		public class Handler : IRequestHandler<Request, Result>
		{
			private readonly UserManager<User> _userManager;
			private readonly ILogger _logger;

			public Handler(UserManager<User> userManager, ILogger<RequestResetPasswordCommand> logger)
			{
				_userManager = userManager;
				_logger = logger;
			}

			public async Task<Result> Handle(Request request, CancellationToken cancellationToken)
			{
				var user = await _userManager.FindByIdAsync(request.UserId.ToString()!);
				if (user == null)
				{
					_logger.LogWarning("Password reset requested for user with id '{UserId}' that does not exist", request.UserId);
					return new Result(ResultStatus.NotFound);
				}

				return new Result(ResultStatus.Success)
				{
					UserId = user.Id,
					Token = await _userManager.GeneratePasswordResetTokenAsync(user)
				};
			}
		}
	}
}

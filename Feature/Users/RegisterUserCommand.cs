using Data.Entities;
using Feature.Framework;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Feature.Users
{
	public class RegisterUserCommand
	{
		public record Request : IRequest<Result>
		{
			[Required]
			public required Guid? UserId { get; init; }

			[Required(AllowEmptyStrings = false)]
			public required string RegisterToken { get; init; }

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
			InvalidPassword,
			InvalidToken
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

				var passwordResult = await _userManager.AddPasswordAsync(user, request.Password);
				if (!passwordResult.Succeeded)
				{
					return new Result(ResultStatus.InvalidPassword);
				}

				var confirmResult = await _userManager.ConfirmEmailAsync(user, request.RegisterToken);
				if (!confirmResult.Succeeded)
				{
					return new Result(ResultStatus.InvalidToken);
				}

				return new Result(ResultStatus.Success);
			}
		}
	}
}

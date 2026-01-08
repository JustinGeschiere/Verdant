using Feature.Framework;
using MediatR;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;

namespace Feature.User
{
	public class CreateUserCommand
	{
		public class Request : IRequest<Result>
		{
			[Required(AllowEmptyStrings = false)]
			public string EmailAddress { get; set; } = null!;
		}

		public class Result(ResultStatus status) : BaseResult<ResultStatus>(status)
		{ }

		public enum ResultStatus
		{
			Success,
			NotFound,
			GeneralError
		}

		public class Handler : IRequestHandler<Request, Result>
		{
			private readonly ILogger _logger;

			public Handler(ILogger<CreateUserCommand> logger)
			{
				_logger = logger;
			}

			public Task<Result> Handle(Request request, CancellationToken cancellationToken)
			{
				_logger.LogInformation("Creating user with e-mail address '{EmailAddress}'.", request.EmailAddress);
				return Task.FromResult(new Result(ResultStatus.Success));
			}
		}
	}
}

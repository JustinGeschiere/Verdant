using Application.Common.Results;
using Mediator;

namespace Application.Features.Users.RequestResetPassword
{
	public record RequestResetPasswordCommand : ICommand<Result<RequestResetPasswordResult>>
	{
		public Guid UserId { get; init; }
	}
}

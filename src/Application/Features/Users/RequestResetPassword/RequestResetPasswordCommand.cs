using Application.Common.Results;
using Mediator;

namespace Application.Features.Users.RequestResetPassword
{
	public record RequestResetPasswordCommand : IRequest<Result<RequestResetPasswordResult>>
	{
		public Guid UserId { get; init; }
	}
}

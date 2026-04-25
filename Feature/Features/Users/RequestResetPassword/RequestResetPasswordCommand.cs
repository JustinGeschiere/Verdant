using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.RequestResetPassword
{
	public record RequestResetPasswordCommand : IRequest<Result<RequestResetPasswordResult>>
	{
		public Guid UserId { get; init; }
	}
}

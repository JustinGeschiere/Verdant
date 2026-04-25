using Application.Common.Results;
using MediatR;

namespace Application.Features.Users.ConfirmResetPassword
{
	public sealed record ConfirmResetPasswordCommand : IRequest<Result>
	{
		public Guid UserId { get; init; }

		public string Token { get; init; } = null!;

		public string Password { get; init; } = null!;

		public string ConfirmPassword { get; init; } = null!;
	}
}

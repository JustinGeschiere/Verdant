using Application.Common.Results;
using Mediator;

namespace Application.Features.Users.InviteUser
{
	public sealed record InviteUserCommand : IRequest<Result<InviteUserResult>>
	{
		public string Email { get; init; } = null!;
	}
}

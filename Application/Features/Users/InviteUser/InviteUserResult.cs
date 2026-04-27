namespace Application.Features.Users.InviteUser
{
	public sealed record InviteUserResult(Guid userId, string token, bool resend)
	{
		public Guid UserId { get; init; } = userId;

		public string Token { get; init; } = token;

		public bool Resend { get; init; } = resend;
	}
}

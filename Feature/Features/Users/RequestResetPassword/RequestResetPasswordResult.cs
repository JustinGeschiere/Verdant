namespace Application.Features.Users.RequestResetPassword
{
	public sealed record RequestResetPasswordResult(Guid userId, string token)
	{
		public Guid UserId { get; init; } = userId;

		public string Token { get; init; } = token;
	}
}

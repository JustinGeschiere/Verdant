using FluentValidation;

namespace Application.Features.Users.InviteUser
{
	public sealed class InviteUserValidator : AbstractValidator<InviteUserCommand>
	{
		public InviteUserValidator()
		{
			RuleFor(i => i.Email)
				.NotEmpty()
				.EmailAddress();
		}
	}
}

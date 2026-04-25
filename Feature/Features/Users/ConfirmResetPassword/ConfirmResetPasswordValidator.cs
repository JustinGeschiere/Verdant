using FluentValidation;

namespace Application.Features.Users.ConfirmResetPassword
{
	public sealed class ConfirmResetPasswordValidator : AbstractValidator<ConfirmResetPasswordCommand>
	{
		public ConfirmResetPasswordValidator()
		{
			RuleFor(i => i.Token)
				.NotEmpty();

			RuleFor(i => i.Password)
				.NotEmpty();

			RuleFor(i => i.ConfirmPassword)
				.NotEmpty();
		}
	}
}

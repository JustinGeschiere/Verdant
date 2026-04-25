using Application.Common.Errors;
using Application.Features.Users.ConfirmResetPassword;
using Application.Features.Users.InviteUser;
using Domain.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Integration.Users
{
	public class InviteUserCommandTests : VerdantFixture
	{
		[Test]
		public async Task NoUser_ReturnsSuccess()
		{
			// Arrange
			var userEmail = "tester@test.com";
			var request = new InviteUserCommand()
			{
				Email = userEmail
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value?.UserId, Is.Not.Null);
				Assert.That(result.Value?.Token, Is.Not.Null.And.Not.WhiteSpace);
				Assert.That(result.Value?.Resend, Is.False);

				await ScopeAsync(async services =>
				{
					var userManager = services.GetRequiredService<UserManager<User>>();
					var createdUser = userManager.FindByEmailAsync(userEmail);

					Assert.That(createdUser, Is.Not.Null);
				});
			}
		}

		[Test]
		public async Task NonEmailConfirmedUser_ReturnsSuccessWithResend()
		{
			// Arrange
			var request = new InviteUserCommand()
			{
				Email = "tester@test.com"
			};

			var arrangeResult = await SendAsync(request);

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value?.UserId, Is.Not.Null);
				Assert.That(result.Value?.Token, Is.Not.Null.And.Not.WhiteSpace);
				Assert.That(result.Value?.Resend, Is.True);

				Assert.That(result.Value!.Token, Is.Not.EqualTo(arrangeResult.Value!.Token));
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsAlreadyExists()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{
				Email = "tester@test.com"
			};

			var inviteResult = await SendAsync(inviteRequest);

			var registerRequest = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var registerResult = await SendAsync(registerRequest);

			// Act
			var result = await SendAsync(inviteRequest);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.NotUnique));
			}
		}
	}
}

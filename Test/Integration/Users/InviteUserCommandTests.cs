using Domain.Users;
using Feature.Users;
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
			var request = new InviteUserCommand.Request()
			{
				Email = userEmail
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(InviteUserCommand.ResultStatus.Success));
				Assert.That(result.UserId, Is.Not.Null);
				Assert.That(result.Token, Is.Not.Null.And.Not.WhiteSpace);

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
			var request = new InviteUserCommand.Request()
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
				Assert.That(result.Status, Is.EqualTo(InviteUserCommand.ResultStatus.SuccessWithResend));
				Assert.That(result.UserId, Is.Not.Null);
				Assert.That(result.Token, Is.Not.Null.And.Not.WhiteSpace);

				Assert.That(result.Token, Is.Not.EqualTo(arrangeResult.Token));
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsAlreadyExists()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{
				Email = "tester@test.com"
			};

			var inviteResult = await SendAsync(inviteRequest);

			var registerRequest = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
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
				Assert.That(result.Status, Is.EqualTo(InviteUserCommand.ResultStatus.AlreadyExists));
			}
		}
	}
}

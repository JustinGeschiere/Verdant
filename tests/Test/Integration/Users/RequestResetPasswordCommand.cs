using Application.Common.Errors;
using Application.Features.Users.ConfirmResetPassword;
using Application.Features.Users.InviteUser;
using Application.Features.Users.RequestResetPassword;
using IntegrationTests;

namespace IntegrationTests.Integration.Users
{
	public class RequestResetPasswordCommandTests : VerdantFixture
	{
		[Test]
		public async Task InvitedUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new RequestResetPasswordCommand()
			{ UserId = inviteResult.Value!.UserId };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value?.UserId, Is.Not.Null);
				Assert.That(result.Value?.Token, Is.Not.Null.And.Not.WhiteSpace);
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var registerRequest = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var registerResult = await SendAsync(registerRequest);

			var request = new RequestResetPasswordCommand()
			{ UserId = inviteResult.Value!.UserId };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);
				Assert.That(result.Value?.UserId, Is.Not.Null);
				Assert.That(result.Value?.Token, Is.Not.Null.And.Not.WhiteSpace);
			}
		}

		[Test]
		public async Task NoUser_ReturnsNotFound()
		{
			// Arrange
			var request = new RequestResetPasswordCommand()
			{ UserId = Guid.NewGuid() };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
			}
		}
	}
}

using Application.Features.Users.ConfirmResetPassword;
using Application.Features.Users.InviteUser;
using Application.Features.Users.RequestResetPassword;

namespace Test.Integration.Users
{
	public class RequestResetPasswordCommandTests : VerdantFixture
	{
		[Test]
		public async Task InvitedUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new RequestResetPasswordHandler.Request()
			{ UserId = inviteResult.UserId };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(RequestResetPasswordHandler.ResultStatus.Success));
				Assert.That(result.UserId, Is.Not.Null);
				Assert.That(result.Token, Is.Not.Null.And.Not.WhiteSpace);
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var registerRequest = new ConfirmResetPasswordHandler.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var registerResult = await SendAsync(registerRequest);

			var request = new RequestResetPasswordHandler.Request()
			{ UserId = inviteResult.UserId };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(RequestResetPasswordHandler.ResultStatus.Success));
				Assert.That(result.UserId, Is.Not.Null);
				Assert.That(result.Token, Is.Not.Null.And.Not.WhiteSpace);
			}
		}

		[Test]
		public async Task NoUser_ReturnsNotFound()
		{
			// Arrange
			var request = new RequestResetPasswordHandler.Request()
			{ UserId = Guid.NewGuid() };

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(RequestResetPasswordHandler.ResultStatus.NotFound));
			}
		}
	}
}

using Domain.Users;
using Feature.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Integration.Users
{
	public class ConfirmResetPasswordCommandTests : VerdantFixture
	{
		[Test]
		public async Task InvitedUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.Success));

				await ScopeAsync(async services =>
				{
					var userManager = services.GetRequiredService<UserManager<User>>();
					var user = await userManager.FindByIdAsync(inviteResult.UserId.ToString()!);
					Assert.That(await userManager.HasPasswordAsync(user!), Is.True);
				});
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var confirmRequest = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var confirmResult = await SendAsync(confirmRequest);

			var tokenRequest = new RequestResetPasswordCommand.Request()
			{ UserId = inviteResult.UserId };

			var tokenResult = await SendAsync(tokenRequest);

			var request = new ConfirmResetPasswordCommand.Request()
			{
				UserId = tokenResult.UserId,
				Token = tokenResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.Success));

				await ScopeAsync(async services =>
				{
					var userManager = services.GetRequiredService<UserManager<User>>();
					var user = await userManager.FindByIdAsync(inviteResult.UserId.ToString()!);
					Assert.That(await userManager.HasPasswordAsync(user!), Is.True);
				});
			}
		}

		[Test]
		public async Task NoUser_ReturnsNotFound()
		{
			// Arrange
			var request = new ConfirmResetPasswordCommand.Request()
			{ 
				UserId = Guid.NewGuid(),
				Token = "someToken",
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.NotFound));
			}
		}

		[Test]
		public async Task InvalidToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = "someToken",
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.InvalidToken));
			}
		}

		[Test]
		public async Task AlreadyConsumedToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var confirmRequest = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var confirmResult = await SendAsync(confirmRequest);

			// Act
			var result = await SendAsync(confirmRequest);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.InvalidToken));
			}
		}

		[Test]
		public async Task ExpiredToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Wait 6 seconds to expire the token (5 second lifetime)
			await Task.Delay(6000);

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.InvalidToken));
			}
		}

		[TestCase("")]
		[TestCase("P@$$w0!")]
		[TestCase("12345678")]
		[TestCase("abcdefg1@")]
		public async Task InvalidPassword_ReturnsInvalidPassword(string password)
		{
			// Arrange
			var inviteRequest = new InviteUserCommand.Request()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand.Request()
			{
				UserId = inviteResult.UserId,
				Token = inviteResult.Token!,
				Password = password,
				ConfirmPassword = password
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(ConfirmResetPasswordCommand.ResultStatus.InvalidPassword));
			}
		}
	}
}

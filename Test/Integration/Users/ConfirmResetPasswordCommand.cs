using Application.Common.Errors;
using Application.Features.Users.ConfirmResetPassword;
using Application.Features.Users.InviteUser;
using Application.Features.Users.RequestResetPassword;
using Domain.Users;
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
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);

				await ScopeAsync(async services =>
				{
					var userManager = services.GetRequiredService<UserManager<User>>();
					var user = await userManager.FindByIdAsync(inviteResult.Value!.UserId.ToString()!);
					Assert.That(await userManager.HasPasswordAsync(user!), Is.True);
				});
			}
		}

		[Test]
		public async Task ExistingUser_ReturnsSuccess()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var confirmRequest = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			var confirmResult = await SendAsync(confirmRequest);

			var tokenRequest = new RequestResetPasswordCommand()
			{ UserId = inviteResult.Value!.UserId };

			var tokenResult = await SendAsync(tokenRequest);

			var request = new ConfirmResetPasswordCommand()
			{
				UserId = tokenResult.Value!.UserId,
				Token = tokenResult.Value!.Token,
				Password = "P@$$w0rd!",
				ConfirmPassword = "P@$$w0rd!"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.True);

				await ScopeAsync(async services =>
				{
					var userManager = services.GetRequiredService<UserManager<User>>();
					var user = await userManager.FindByIdAsync(inviteResult.Value!.UserId.ToString()!);
					Assert.That(await userManager.HasPasswordAsync(user!), Is.True);
				});
			}
		}

		[Test]
		public async Task NoUser_ReturnsNotFound()
		{
			// Arrange
			var request = new ConfirmResetPasswordCommand()
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
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.NotFound));
			}
		}

		[Test]
		public async Task InvalidToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
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
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.InvalidToken));
			}
		}

		[Test]
		public async Task AlreadyConsumedToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var confirmRequest = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token!,
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
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.InvalidToken));
			}
		}

		[Test]
		public async Task ExpiredToken_ReturnsInvalidToken()
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
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
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.InvalidToken));
			}
		}

		[TestCase("P@$$w0!")]
		[TestCase("12345678")]
		[TestCase("abcdefg1@")]
		public async Task InvalidPassword_ReturnsInvalidPassword(string password)
		{
			// Arrange
			var inviteRequest = new InviteUserCommand()
			{ Email = "tester@test.com" };

			var inviteResult = await SendAsync(inviteRequest);

			var request = new ConfirmResetPasswordCommand()
			{
				UserId = inviteResult.Value!.UserId,
				Token = inviteResult.Value!.Token,
				Password = password,
				ConfirmPassword = password
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.IsSuccess, Is.False);
				Assert.That(result.Error, Is.EqualTo(UserErrors.InvalidPassword));
			}
		}
	}
}

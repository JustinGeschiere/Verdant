using Feature.User;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace Test.Integration.User
{
	public class CreateUserCommandTests : VerdantFixture
	{
		[Test]
		public async Task ValidRequest_ReturnsSuccess()
		{
			// Arrange
			var request = new CreateUserCommand.Request()
			{
				EmailAddress = "tester@test.com"
			};

			// Act
			var result = await SendAsync(request);

			// Assert
			using (Assert.EnterMultipleScope())
			{
				Assert.That(result, Is.Not.Null);
				Assert.That(result.Status, Is.EqualTo(CreateUserCommand.ResultStatus.Success));
			}
		}
	}
}

using Data;
using Data.Entities;
using Feature.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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

		[Test]
		public async Task Database_CanAddEntry()
		{
			// Arrange
			var plantName = Guid.NewGuid().ToString();

			// Act
			await ScopeAsync(async services =>
			{
				var context = services.GetRequiredService<VerdantContext>();

				var plantName = Guid.NewGuid().ToString();
				var plant = new Plant()
				{
					GivenName = plantName,
					ScientificName = Guid.NewGuid().ToString(),
					WinterWateringInterval = TimeSpan.FromDays(14),
					SummerWateringInterval = TimeSpan.FromDays(7),
				};

				context.Plants.Add(plant);
				await context.SaveChangesAsync();
			});

			// Assert
			await ScopeAsync(async services =>
			{
				var context = services.GetRequiredService<VerdantContext>();
				var plant = await context.Plants.AsNoTracking().Where(i => i.GivenName.Equals(plantName)).FirstOrDefaultAsync();

				using (Assert.EnterMultipleScope())
				{
					Assert.That(plant, Is.Not.Null);
					Assert.That(plant!.GivenName, Is.EqualTo(plantName));

					Assert.That(context.Plants.Count(), Is.EqualTo(1));
				}
			});
		}
	}
}

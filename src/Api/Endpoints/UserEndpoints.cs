using Mediator;

namespace Api.Endpoints
{
	public static class UserEndpoints
	{
		public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
		{
			var groupBuilder = builder.MapGroup("/user")
				.WithTags("User");

			groupBuilder.MapPost("/test", Test)
				.WithName("Test")
				.WithTags("User");

			return builder;
		}

		private static IResult Test([AsParameters] string payload, [FromKeyedServices] IMediator mediator)
		{
			return Results.Ok();
		}
	}
}

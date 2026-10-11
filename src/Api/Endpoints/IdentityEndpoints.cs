using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints
{
	public static class IdentityEndpoints
	{
		public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
		{
			var groupBuilder = builder.MapGroup("/identity")
				.WithTags("Identity");

			groupBuilder.MapPost("/test", Test)
				.WithName("Test")
				.WithTags("Identity");

			return builder;
		}

		private static IResult Test([AsParameters] string payload, [FromKeyedServices] IMediator mediator)
		{
			return Results.Ok();
		}
	}
}

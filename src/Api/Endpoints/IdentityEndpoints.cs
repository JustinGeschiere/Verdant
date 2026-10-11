using Microsoft.AspNetCore.Http.HttpResults;

namespace Api.Endpoints
{
	public static class IdentityEndpoints
	{
		public static IEndpointRouteBuilder MapEndpoints(IEndpointRouteBuilder builder)
		{
			return builder;
		}

		private Results<NoContent, BadRequest> Test(IMediator mediator)
		{

		}
	}
}

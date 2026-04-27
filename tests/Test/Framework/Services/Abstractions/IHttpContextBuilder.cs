using System.Security.Claims;

namespace IntegrationTests.Framework.Services.Abstractions
{
	public interface IHttpContextBuilder
	{
		IHttpContextBuilder WithUser(ClaimsPrincipal user);

		IHttpContextBuilder WithHeader(string name, string value);

		void Apply();

		void Clear();
	}
}

using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Test.Framework.Services.Abstractions
{
	public interface IHttpContextBuilder
	{
		IHttpContextBuilder WithUser(ClaimsPrincipal user);

		IHttpContextBuilder WithHeader(string name, string value);

		void Apply();

		void Clear();
	}
}

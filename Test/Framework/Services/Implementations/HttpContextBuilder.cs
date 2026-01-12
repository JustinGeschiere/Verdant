using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Test.Framework.Services.Abstractions;

namespace Test.Framework.Services.Implementations
{
	internal class HttpContextBuilder : IHttpContextBuilder
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private DefaultHttpContext _context;

		public HttpContextBuilder(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
			_context = new DefaultHttpContext();
		}

		public IHttpContextBuilder WithUser(ClaimsPrincipal user)
		{
			_context.User = user;
			return this;
		}

		public IHttpContextBuilder WithHeader(string name, string value)
		{
			_context.Request.Headers[name] = value;
			return this;
		}

		/// <summary>
		/// Assigns the built HttpContext to the IHttpContextAccessor service.
		/// </summary>
		public void Apply()
		{
			_httpContextAccessor.HttpContext = _context;
		}

		public void Clear()
		{
			_context = new DefaultHttpContext();
			Apply();
		}
	}
}

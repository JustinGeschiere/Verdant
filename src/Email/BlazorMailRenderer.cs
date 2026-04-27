using Email.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Email
{
	internal class BlazorMailRenderer : IMailRenderer
	{
		private readonly IServiceProvider _serviceProvider;

		public BlazorMailRenderer(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public async Task<string> RenderAsync<TComponent>(IMailTemplate<TComponent> template)
			where TComponent : IComponent
		{
			await using var scope = _serviceProvider.CreateAsyncScope();

			var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
			var renderer = new HtmlRenderer(scope.ServiceProvider, loggerFactory);

			return await renderer.Dispatcher.InvokeAsync(async () =>
			{
				var render = await renderer.RenderComponentAsync<TComponent>(template.ToParameters());
				return render.ToHtmlString();
			});
		}
	}
}

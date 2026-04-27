using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Logging.Abstractions;
using Email.Abstractions;

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
			var htmlRenderer = new HtmlRenderer(_serviceProvider, NullLoggerFactory.Instance);

			var result = await htmlRenderer.Dispatcher.InvokeAsync(() =>
				htmlRenderer.RenderComponentAsync<TComponent>(template.ToParameters()));

			return result.ToHtmlString();
		}
	}
}

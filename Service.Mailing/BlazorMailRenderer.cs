using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Service.Mailing
{
    internal class BlazorMailRenderer
    {
        private readonly IServiceProvider _serviceProvider;

        public BlazorMailRenderer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> RenderAsync<TComponent>(IDictionary<string, object?> parameters)
            where TComponent : IComponent
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var loggerFactory = scope.ServiceProvider.GetRequiredService<ILoggerFactory>();
            var renderer = new HtmlRenderer(scope.ServiceProvider, loggerFactory);

            var result = await renderer.Dispatcher.InvokeAsync(() =>
                renderer.RenderComponentAsync<TComponent>(ParameterView.FromDictionary(parameters)));

            return result.ToHtmlString();
        }
    }
}

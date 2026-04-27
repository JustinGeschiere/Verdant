using Microsoft.AspNetCore.Components;

namespace Email.Abstractions
{
    public interface IMailRenderer
    {
        Task<string> RenderAsync<TComponent>(IMailTemplate<TComponent> template)
            where TComponent : IComponent;
    }
}

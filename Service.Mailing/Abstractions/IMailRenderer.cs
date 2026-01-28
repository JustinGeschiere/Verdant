using Microsoft.AspNetCore.Components;

namespace Service.Mailing.Abstractions
{
    public interface IMailRenderer
    {
        Task<string> RenderAsync<TComponent>(IMailTemplate<TComponent> template)
            where TComponent : IComponent;
    }
}

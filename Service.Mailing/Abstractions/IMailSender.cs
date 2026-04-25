using Microsoft.AspNetCore.Components;

namespace Service.Mailing.Abstractions
{
    public interface IMailSender
    {
        Task<bool> SendTemplateAsync<TComponent>(string receiver, IMailTemplate<TComponent> template)
            where TComponent : IComponent;
    }
}

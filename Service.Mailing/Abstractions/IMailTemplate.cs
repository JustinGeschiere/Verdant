using Microsoft.AspNetCore.Components;

namespace Service.Mailing.Abstractions
{
    public interface IMailTemplate<TComponent>
        where TComponent : IComponent
    {
        string GetSubject();

        ParameterView ToParameters();
    }
}

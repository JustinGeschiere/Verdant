using Microsoft.AspNetCore.Components;

namespace Email.Abstractions
{
    public interface IMailTemplate<TComponent>
        where TComponent : IComponent
    {
        string GetSubject();

        ParameterView ToParameters();
    }
}

using Microsoft.AspNetCore.Components;
using Email.Abstractions;
using Email.Components.Mails;

namespace Email.Templates
{
    public class WelcomeUserTemplate : IMailTemplate<WelcomeUser>
    {
        public required string FirstName { get; set; } = default!;
        public required string RegisterToken { get; set; } = default!;

        public string GetSubject() => "Welcome to Verdant!";

        public ParameterView ToParameters()
        {
            return ParameterView.FromDictionary(new Dictionary<string, object?>
            {
                [nameof(FirstName)] = FirstName,
                [nameof(RegisterToken)] = RegisterToken
            });
        }
    }
}

using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.Mailing.Abstractions;
using Service.Mailing.Options;

namespace Service.Mailing
{
    internal class MailKitMailSender : IMailSender
    {
        private readonly SmtpOptions _smtpOptions;
        private readonly IMailRenderer _mailRenderer;
        private readonly ILogger _logger;

        public MailKitMailSender(IOptions<SmtpOptions> smtpOptions, IMailRenderer mailRenderer, ILogger<MailKitMailSender> logger)
        {
            _smtpOptions = smtpOptions.Value;
            _mailRenderer = mailRenderer;
            _logger = logger;
        }

        public async Task<bool> SendTemplateAsync<TComponent>(string receiver, IMailTemplate<TComponent> template)
            where TComponent : IComponent
        {
            var body = await _mailRenderer.RenderAsync(template);
            return await SendHtmlAsync(receiver, template.GetSubject(), body);
        }

        private async Task<bool> SendHtmlAsync(string receiver, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_smtpOptions.FromName, _smtpOptions.FromEmail));
            message.To.Add(MailboxAddress.Parse(receiver));
            message.Subject = subject;

            message.Body = new BodyBuilder()
            {
                HtmlBody = body
            }.ToMessageBody();

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port,
                    _smtpOptions.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls);

                if (!string.IsNullOrEmpty(_smtpOptions.Username))
                {
                    await client.AuthenticateAsync(
                        _smtpOptions.Username,
                        _smtpOptions.Password);
                }

                await client.SendAsync(message);
                await client.DisconnectAsync(true);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Failed to send Html e-mail to address '{Receiver}'", receiver);
                return false;
            }

            return true;
        }
    }
}

using Infrastructure.Composition.Options;
using Infrastructure.Mailing.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Infrastructure.Mailing
{
    internal class MailKitMailSender : IMailSender
    {
        private readonly SmtpOptions _smtpOptions;
        private readonly ILogger _logger;

        public MailKitMailSender(IOptions<SmtpOptions> smtpOptions, ILogger<MailKitMailSender> logger)
        {
            _smtpOptions = smtpOptions.Value;
            _logger = logger;
        }

        public async Task<bool> SendHtmlAsync(string receiver, string subject, string body)
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
                await client.ConnectAsync(_smtpOptions.Host, _smtpOptions.Port, SecureSocketOptions.Auto);

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

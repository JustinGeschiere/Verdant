using Microsoft.Extensions.Options;
using Service.Mailing.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.Mailing
{
    internal class MailKitMailSender
    {
        private readonly SmtpOptions _smtpOptions;

        public MailKitMailSender(IOptions<SmtpOptions> smtpOptions)
        {
            _smtpOptions = smtpOptions.Value;
        }

        public async Task SendMailAsync()
        {

        }
    }
}

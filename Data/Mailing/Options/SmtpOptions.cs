using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Mailing.Options
{
    public class SmtpOptions
    {
        public const string SECTION = "Smtp";

        [Required]
        public string Host { get; set; } = default!;

        [Range(1, 65535)]
        public int Port { get; set; }

        public bool UseSsl { get; set; }

        public string Username { get; set; } = default!;

        public string Password { get; set; } = default!;

        [Required, EmailAddress]
        public string FromEmail { get; set; } = default!;

        [Required]
        public string FromName { get; set; } = default!;
    }
}

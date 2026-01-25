using System.ComponentModel.DataAnnotations;

namespace Service.Mailing.Options
{
    public class SmtpOptions
    {
        public const string SECTION = "Smtp";

        [Required]
        public string Host { get; set; } = default!;

        [Range(1, 65535)]
        public int Port { get; set; }

        public bool UseSsl { get; set; }

        [Required]
        public string Username { get; set; } = default!;

        [Required]
        public string Password { get; set; } = default!;

        [Required, EmailAddress]
        public string From { get; set; } = default!;
    }
}

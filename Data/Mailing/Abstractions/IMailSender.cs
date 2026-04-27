namespace Infrastructure.Mailing.Abstractions
{
	public interface IMailSender
	{
		Task<bool> SendHtmlAsync(string receiver, string subject, string body);
	}
}

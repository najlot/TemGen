namespace Todo.Service.Shared.Configuration;

public class SmtpConfiguration
{
	public string Host { get; set; } = "";
	public int Port { get; set; } = 587;
	public bool UseSsl { get; set; } = true;
	public string Username { get; set; } = "";
	public string Password { get; set; } = "";
	public string MailFromAddress { get; set; } = "";
	public string MailFromName { get; set; } = "";
}
namespace Todo.Contracts.Auth;

public class ResetPassword
{
	public string EMail { get; set; } = string.Empty;
	public string Code { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}
namespace <# Project.Namespace#>.Contracts.Auth;

public class AuthRequest
{
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
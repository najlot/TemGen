namespace <#cs Write(Project.Namespace)#>.Contracts;

public class AuthRequest
{
	public string Username { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
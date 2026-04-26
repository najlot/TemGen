namespace <# Project.Namespace#>.Contracts.Auth;

public class RequestPasswordReset
{
	public string EMail { get; set; } = string.Empty;
	public string Culture { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
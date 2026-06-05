using System;

namespace <# Project.Namespace#>.Contracts.Users;

public class UserCreated
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
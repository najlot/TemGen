using System;

namespace <# Project.Namespace#>.Contracts.Users;

public class UserUpdated
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
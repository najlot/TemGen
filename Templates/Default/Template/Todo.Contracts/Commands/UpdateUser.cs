using System;

namespace <# Project.Namespace#>.Contracts.Commands;

public class UpdateUser
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
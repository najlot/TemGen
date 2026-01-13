using System;

namespace <#cs Write(Project.Namespace)#>.Contracts.ListItems;

public sealed class UserListItem
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
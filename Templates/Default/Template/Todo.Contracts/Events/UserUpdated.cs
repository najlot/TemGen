using System;

namespace <#cs Write(Project.Namespace)#>.Contracts.Events;

public class UserUpdated(
	Guid id,
	string username,
	string eMail)
{
	public Guid Id { get; } = id;
	public string Username { get; } = username;
	public string EMail { get; } = eMail;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
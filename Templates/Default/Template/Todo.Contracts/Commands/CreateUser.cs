using System;

namespace <#cs Write(Project.Namespace)#>.Contracts.Commands;

public class CreateUser(
	Guid id,
	string username,
	string eMail,
	string password)
{
	public Guid Id { get; } = id;
	public string Username { get; } = username;
	public string EMail { get; } = eMail;
	public string Password { get; } = password;
}<#cs SetOutputPathAndSkipOtherDefinitions()#>
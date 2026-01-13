using System;

namespace Todo.Contracts.Commands;

public class UpdateUser(
	Guid id,
	string username,
	string eMail,
	string password)
{
	public Guid Id { get; } = id;
	public string Username { get; } = username;
	public string EMail { get; } = eMail;
	public string Password { get; } = password;
}
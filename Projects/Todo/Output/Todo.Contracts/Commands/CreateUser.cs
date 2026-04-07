using System;

namespace Todo.Contracts.Commands;

public class CreateUser
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
	public string Password { get; set; } = string.Empty;
}
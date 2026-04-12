using System;

namespace Todo.Contracts.Users;

public sealed class UserListItem
{
	public Guid Id { get; set; }
	public string Username { get; set; } = string.Empty;
	public string EMail { get; set; } = string.Empty;
}
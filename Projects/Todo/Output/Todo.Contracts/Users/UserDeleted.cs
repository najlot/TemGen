using System;

namespace Todo.Contracts.Users;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}
using System;

namespace Todo.Contracts.Events;

public class UserDeleted(Guid id)
{
	public Guid Id { get; } = id;
}
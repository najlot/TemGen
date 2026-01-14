using System;

namespace Todo.Contracts.Events;

public class TodoItemDeleted(Guid id)
{
	public Guid Id { get; } = id;
}
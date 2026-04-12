using System;

namespace Todo.Contracts.TodoItems;

public class TodoItemDeleted(Guid id)
{
	public Guid Id { get; } = id;
}
using System;
using System.Collections.Generic;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public class TodoItemListItemModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
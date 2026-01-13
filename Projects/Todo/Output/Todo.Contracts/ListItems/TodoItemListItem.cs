using System;
using System.Collections.Generic;

namespace Todo.Contracts.ListItems;

public sealed class TodoItemListItem
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
}
using System;
using System.Collections.Generic;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public class ChecklistTaskModel
{
	public int Id { get; set; }

	public bool IsDone { get; set; }
	public string Description { get; set; } = string.Empty;
}
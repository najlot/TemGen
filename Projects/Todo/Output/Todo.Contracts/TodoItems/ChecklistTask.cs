using System;
using System.Collections.Generic;

namespace Todo.Contracts.TodoItems;

public class ChecklistTask
{
	public int Id { get; set; }
	public bool IsDone { get; set; }
	public string Description { get; set; } = string.Empty;

}
using System;
using System.Collections.Generic;

namespace Todo.Contracts.TodoItems;

public class TodoItemCreated
{
	public Guid Id { get; set; }
	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public Guid AssignedToId { get; set; }
	public TodoItemStatus Status { get; set; }
	public DateTime DueDate { get; set; }
	public string Priority { get; set; } = string.Empty;
	public List<ChecklistTask> Checklist { get; set; } = [];
}
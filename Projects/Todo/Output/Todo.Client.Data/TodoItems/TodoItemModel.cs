using System;
using System.Collections.Generic;
using Todo.Contracts.TodoItems;

namespace Todo.Client.Data.TodoItems;

public class TodoItemModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public Guid AssignedToId { get; set; }
	public TodoItemStatus Status { get; set; }
	public DateTime DueDate { get; set; }
	public string Priority { get; set; } = string.Empty;
	public List<ChecklistTaskModel> Checklist { get; set; } = [];
}
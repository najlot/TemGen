using System;
using System.Collections.Generic;
using Todo.Contracts;

namespace Todo.Client.Data.Models;

public class TodoItemModel
{
	public Guid Id { get; set; }

	public string Title { get; set; } = string.Empty;
	public string Content { get; set; } = string.Empty;
	public DateTime CreatedAt { get; set; }
	public string CreatedBy { get; set; } = string.Empty;
	public Guid AssignedToId { get; set; }
	public TodoItemStatus Status { get; set; }
	public DateTime ChangedAt { get; set; }
	public string ChangedBy { get; set; } = string.Empty;
	public string Priority { get; set; } = string.Empty;
	public List<ChecklistTaskModel> Checklist { get; set; } = [];
}
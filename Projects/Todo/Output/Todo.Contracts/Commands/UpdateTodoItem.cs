using System;
using System.Collections.Generic;

namespace Todo.Contracts.Commands;

public class UpdateTodoItem(
	Guid id,
	string title,
	string content,
	DateTime createdAt,
	string createdBy,
	Guid assignedToId,
	TodoItemStatus status,
	DateTime changedAt,
	string changedBy,
	string priority,
	List<ChecklistTask> checklist)
{
	public Guid Id { get; } = id;
	public string Title { get; } = title;
	public string Content { get; } = content;
	public DateTime CreatedAt { get; } = createdAt;
	public string CreatedBy { get; } = createdBy;
	public Guid AssignedToId { get; } = assignedToId;
	public TodoItemStatus Status { get; } = status;
	public DateTime ChangedAt { get; } = changedAt;
	public string ChangedBy { get; } = changedBy;
	public string Priority { get; } = priority;
	public List<ChecklistTask> Checklist { get; } = checklist;
}
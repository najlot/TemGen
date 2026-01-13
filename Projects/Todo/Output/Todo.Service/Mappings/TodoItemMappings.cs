using Najlot.Map;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Service.Model;

namespace Todo.Service.Mappings;

internal class TodoItemMappings
{
	public TodoItemCreated MapToCreated(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedTo,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			from.Checklist);

	public TodoItemUpdated MapToUpdated(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedTo,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			from.Checklist);

	public void MapToModel(IMap map, CreateTodoItem from, TodoItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = from.Checklist;
	}

	public void MapToModel(IMap map, UpdateTodoItem from, TodoItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList(to.Checklist);
	}

	public void MapToModel(IMap map, TodoItemModel from, TodoItem to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedTo = from.AssignedTo;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = from.Checklist;
	}

	public void MapToModel(IMap map, TodoItemModel from, TodoItemListItem to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}
}
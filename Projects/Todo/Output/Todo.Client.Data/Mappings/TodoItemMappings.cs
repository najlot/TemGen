using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

internal sealed class TodoItemMappings
{
	public CreateTodoItem MapToCreate(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedToId,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>());

	public TodoItemCreated MapToCreated(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedToId,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>());

	public UpdateTodoItem MapToUpdate(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedToId,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>());

	public TodoItemUpdated MapToUpdated(IMap map, TodoItemModel from) =>
		new(from.Id,
			from.Title,
			from.Content,
			from.CreatedAt,
			from.CreatedBy,
			from.AssignedToId,
			from.Status,
			from.ChangedAt,
			from.ChangedBy,
			from.Priority,
			map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>());

	public void MapToModel(IMap map, TodoItemCreated from, TodoItemListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, TodoItemUpdated from, TodoItemListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, TodoItemListItem from, TodoItemListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapModelToModel(IMap map, TodoItemModel from, TodoItemListItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
	}

	public void MapToModel(IMap map, TodoItem from, TodoItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedToId = from.AssignedToId;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList<ChecklistTaskModel>();
	}

	public void MapToModel(IMap map, TodoItemUpdated from, TodoItemModel to)
	{
		to.Id = from.Id;
		to.Title = from.Title;
		to.Content = from.Content;
		to.CreatedAt = from.CreatedAt;
		to.CreatedBy = from.CreatedBy;
		to.AssignedToId = from.AssignedToId;
		to.Status = from.Status;
		to.ChangedAt = from.ChangedAt;
		to.ChangedBy = from.ChangedBy;
		to.Priority = from.Priority;
		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList<ChecklistTaskModel>();
	}
}
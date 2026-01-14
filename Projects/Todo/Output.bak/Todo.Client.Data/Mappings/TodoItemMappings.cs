using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class TodoItemMappings
{
	public static CreateTodoItem MapToCreate(IMap map, TodoItemModel from) =>
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

	public static TodoItemCreated MapToCreated(IMap map, TodoItemModel from) =>
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

	public static UpdateTodoItem MapToUpdate(IMap map, TodoItemModel from) =>
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

	public static TodoItemUpdated MapToUpdated(IMap map, TodoItemModel from) =>
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

	public static partial void MapToModel(IMap map, TodoItemCreated from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItemUpdated from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItemListItem from, TodoItemListItemModel to);

	public static partial void MapModelToModel(IMap map, TodoItemModel from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItem from, TodoItemModel to);

	public static partial void MapToModel(IMap map, TodoItemUpdated from, TodoItemModel to);
}
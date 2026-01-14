using Najlot.Map;
using Najlot.Map.Attributes;
using System;
using System.Linq.Expressions;
using Todo.Contracts;
using Todo.Contracts.Commands;
using Todo.Contracts.Events;
using Todo.Contracts.ListItems;
using Todo.Service.Model;

namespace Todo.Service.Mappings;

[Mapping]
internal partial class TodoItemMappings
{
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
			from.Checklist);

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
			from.Checklist);

	public static partial void MapToModel(IMap map, CreateTodoItem from, TodoItemModel to);

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToModel(IMap map, UpdateTodoItem from, TodoItemModel to);
	public static void MapToModel(IMap map, UpdateTodoItem from, TodoItemModel to)
	{
		MapPartialToModel(map, from, to);

		to.Checklist = map.From<ChecklistTask>(from.Checklist).ToList(to.Checklist);
	}


	public static partial void MapToModel(IMap map, TodoItemModel from, TodoItem to);

	public static Expression<Func<TodoItemModel, TodoItemListItem>> GetListItemExpression()
	{
		return from => new TodoItemListItem
		{
			Id = from.Id,
			Title = from.Title,
			Content = from.Content
		};
	}

	public static partial void MapToModel(IMap map, TodoItemModel from, TodoItemListItem to);
}
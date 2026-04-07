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
	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToCreate(IMap map, TodoItemModel from, CreateTodoItem to);

	public static void MapToCreate(IMap map, TodoItemModel from, CreateTodoItem to)
	{
		MapPartialToCreate(map, from, to);
		to.Checklist = map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>();
	}

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToCreated(IMap map, TodoItemModel from, TodoItemCreated to);

	public static void MapToCreated(IMap map, TodoItemModel from, TodoItemCreated to)
	{
		MapPartialToCreated(map, from, to);
		to.Checklist = map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>();
	}

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToUpdate(IMap map, TodoItemModel from, UpdateTodoItem to);

	public static void MapToUpdate(IMap map, TodoItemModel from, UpdateTodoItem to)
	{
		MapPartialToUpdate(map, from, to);
		to.Checklist = map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>();
	}

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToUpdated(IMap map, TodoItemModel from, TodoItemUpdated to);

	public static void MapToUpdated(IMap map, TodoItemModel from, TodoItemUpdated to)
	{
		MapPartialToUpdated(map, from, to);
		to.Checklist = map.From<ChecklistTaskModel>(from.Checklist).ToList<ChecklistTask>();
	}

	public static partial void MapToModel(IMap map, TodoItemCreated from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItemUpdated from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItemListItem from, TodoItemListItemModel to);

	public static partial void MapModelToModel(IMap map, TodoItemModel from, TodoItemListItemModel to);

	public static partial void MapToModel(IMap map, TodoItem from, TodoItemModel to);

	public static partial void MapToModel(IMap map, TodoItemUpdated from, TodoItemModel to);
}
using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.TodoItems;
using Todo.Contracts.TodoItems;

namespace Todo.ClientBase.TodoItems;

[Mapping]
internal sealed partial class TodoItemViewModelMappings
{
	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToViewModel(IMap map, TodoItemUpdated from, TodoItemViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, TodoItemUpdated from, TodoItemViewModel to)
	{
		MapPartialToViewModel(map, from, to);

		to.Checklist = [.. map.From<ChecklistTask>(from.Checklist).To<ChecklistTaskViewModel>()];
	}

	public static partial void MapToModel(IMap map, TodoItemViewModel from, TodoItemModel to);

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToViewModel(IMap map, TodoItemModel from, TodoItemViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, TodoItemModel from, TodoItemViewModel to)
	{
		MapPartialToViewModel(map, from, to);

		to.Checklist = [.. map.From<ChecklistTaskModel>(from.Checklist).To<ChecklistTaskViewModel>()];
	}

	public static partial void MapToViewModel(IMap map, TodoItemListItemModel from, TodoItemListItemViewModel to);

	public static partial void MapToViewModel(IMap map, TodoItemCreated from, TodoItemListItemViewModel to);

	public static partial void MapToViewModel(IMap map, TodoItemUpdated from, TodoItemListItemViewModel to);
}
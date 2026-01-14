using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;
using Todo.Contracts.Events;

namespace Todo.ClientBase.Mappings;

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

		foreach (var e in to.Checklist) e.ParentId = from.Id;
	}

	public static partial void MapToModel(IMap map, TodoItemViewModel from, TodoItemModel to);

	[MapIgnoreProperty(nameof(to.Checklist))]
	private static partial void MapPartialToViewModel(IMap map, TodoItemModel from, TodoItemViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, TodoItemModel from, TodoItemViewModel to)
	{
		MapPartialToViewModel(map, from, to);

		to.Checklist = [.. map.From<ChecklistTaskModel>(from.Checklist).To<ChecklistTaskViewModel>()];

		foreach (var e in to.Checklist) e.ParentId = from.Id;
	}

}
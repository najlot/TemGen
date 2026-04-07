using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModels;
using Todo.Contracts;

namespace Todo.ClientBase.Mappings;

[Mapping]
internal sealed partial class ChecklistTaskViewModelMappings
{
	public static partial void MapToModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskModel to);

	[MapIgnoreProperty(nameof(to.ChangeVisitor))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	public static partial void MapFromViewModelToViewModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskViewModel to);

	private static partial void MapPartialToViewModel(IMap map, ChecklistTaskModel from, ChecklistTaskViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, ChecklistTaskModel from, ChecklistTaskViewModel to)
	{
		MapPartialToViewModel(map, from, to);
	}

	private static partial void MapPartialToViewModel(IMap map, ChecklistTask from, ChecklistTaskViewModel to);

	[MapValidateSource]
	public static void MapToViewModel(IMap map, ChecklistTask from, ChecklistTaskViewModel to)
	{
		MapPartialToViewModel(map, from, to);
	}
}
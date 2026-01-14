using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;

namespace Todo.ClientBase.Mappings;

internal sealed class ChecklistTaskViewModelMappings
{
	public static void MapToModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskModel to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}

	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapFromViewModelToViewModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskViewModel to)
	{
		to.Id = from.Id;
		to.ParentId = from.ParentId;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, ChecklistTaskModel from, ChecklistTaskViewModel to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static void MapToViewModel(IMap map, ChecklistTask from, ChecklistTaskViewModel to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}
}
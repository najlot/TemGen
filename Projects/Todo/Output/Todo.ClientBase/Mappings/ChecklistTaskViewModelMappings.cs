using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.ClientBase.ViewModel;
using Todo.Contracts;

namespace Todo.ClientBase.Mappings;

[Mapping]
internal sealed partial class ChecklistTaskViewModelMappings
{
	public static partial void MapToModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskModel to);

	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static partial void MapFromViewModelToViewModel(IMap map, ChecklistTaskViewModel from, ChecklistTaskViewModel to);

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static partial void MapToViewModel(IMap map, ChecklistTaskModel from, ChecklistTaskViewModel to);

	[MapIgnoreProperty(nameof(to.ParentId))]
	[MapIgnoreProperty(nameof(to.HasErrors))]
	[MapIgnoreProperty(nameof(to.Errors))]
	public static partial void MapToViewModel(IMap map, ChecklistTask from, ChecklistTaskViewModel to);
}
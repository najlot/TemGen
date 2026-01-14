using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class ChecklistTaskMappings
{
	public static partial void MapFromModel(IMap map, ChecklistTaskModel from, ChecklistTask to);

	public static partial void MapToModel(IMap map, ChecklistTask from, ChecklistTaskModel to);

	public static partial void MapFromModelToModel(IMap map, ChecklistTaskModel from, ChecklistTaskModel to);
}
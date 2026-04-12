using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Contracts.TodoItems;

namespace Todo.Service.Features.ChecklistTasks;

[Mapping]
internal partial class ChecklistTaskMappings
{
	[MapIgnoreProperty(nameof(to.Id))]
	public static partial void Map(IMap map, ChecklistTask from, ChecklistTask to);
}
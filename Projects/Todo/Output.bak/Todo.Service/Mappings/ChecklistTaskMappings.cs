using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Contracts;

namespace Todo.Service.Mappings;

[Mapping]
internal partial class ChecklistTaskMappings
{
	[MapIgnoreProperty(nameof(to.Id))] // Do not map the Id property as it makes problems with entity tracking in EF Core
	public static partial void Map(IMap map, ChecklistTask from, ChecklistTask to);
}
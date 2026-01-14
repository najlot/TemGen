using Najlot.Map;
using Najlot.Map.Attributes;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Mappings;

[Mapping]
internal sealed partial class ChecklistTaskMappings
{
	[MapIgnoreProperty(nameof(to.Id))] // Do not map the Id property as it makes problems with entity tracking in EF Core
	public static partial void Map(IMap map, ChecklistTask from, ChecklistTask to);
}
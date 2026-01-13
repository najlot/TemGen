using Najlot.Map;
using Todo.Contracts;

namespace Todo.Service.Mappings;

internal class ChecklistTaskMappings
{
	public void Map(IMap map, ChecklistTask from, ChecklistTask to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}
}
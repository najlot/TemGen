using Najlot.Map;
using Todo.Client.Data.Models;
using Todo.Contracts;

namespace Todo.Client.Data.Mappings;

internal sealed class ChecklistTaskMappings
{
	public void MapFromModel(IMap map, ChecklistTaskModel from, ChecklistTask to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}

	public void MapToModel(IMap map, ChecklistTask from, ChecklistTaskModel to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}

	public void MapFromModelToModel(IMap map, ChecklistTaskModel from, ChecklistTaskModel to)
	{
		to.Id = from.Id;
		to.IsDone = from.IsDone;
		to.Description = from.Description;
	}
}
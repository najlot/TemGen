using Najlot.Map;
using Todo.ClientBase.Mappings;

namespace Todo.ClientBase;

public static class MapRegisterExtensions
{
	public static IMap RegisterViewModelMappings(this IMap map)
	{
		map.Register<NoteViewModelMappings>();
		map.Register<ChecklistTaskViewModelMappings>();
		map.Register<TodoItemViewModelMappings>();

		return map;
	}
}
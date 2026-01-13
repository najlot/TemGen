using Najlot.Map;
using Todo.Client.Data.Mappings;

namespace Todo.Client.Data;

public static class MapRegisterExtensions
{
	public static IMap RegisterDataMappings(this IMap map)
	{
		map.Register<NoteMappings>();
		map.Register<ChecklistTaskMappings>();
		map.Register<TodoItemMappings>();
		map.Register<UserMappings>();

		return map;
	}
}
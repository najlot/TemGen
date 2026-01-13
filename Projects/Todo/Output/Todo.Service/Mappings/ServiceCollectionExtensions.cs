using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Mappings;

public static class ServiceCollectionExtensions
{
	public static Najlot.Map.IMap RegisterDataMappings(this Najlot.Map.IMap map)
	{
		map.Register<ChecklistTaskMappings>();
		map.Register<NoteMappings>();
		map.Register<TodoItemMappings>();
		map.Register<UserMappings>();

		return map;
	}

	public static IServiceCollection RegisterDataMappings(this IServiceCollection services)
	{
		var map = new Najlot.Map.Map().RegisterDataMappings();
		return services.AddSingleton(map);
	}
}
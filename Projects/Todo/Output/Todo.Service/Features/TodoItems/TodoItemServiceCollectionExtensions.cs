using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.TodoItems;

public static class TodoItemServiceCollectionExtensions
{
	public static IServiceCollection RegisterTodoItemFeature(this IServiceCollection services)
	{
		services.AddScoped<TodoItemService>();
		return services;
	}
}

using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.Trash;

public static class TrashServiceCollectionExtensions
{
	public static IServiceCollection RegisterTrashFeature(this IServiceCollection services)
	{
		services.AddScoped<TrashService>();
		services.AddScoped<ITrashSource, NoteTrashSource>();
		services.AddScoped<ITrashSource, TodoItemTrashSource>();

		return services;
	}
}

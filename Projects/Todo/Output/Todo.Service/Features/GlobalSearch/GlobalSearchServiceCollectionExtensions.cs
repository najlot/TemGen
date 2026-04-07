using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.GlobalSearch;

public static class GlobalSearchServiceCollectionExtensions
{
	public static IServiceCollection RegisterGlobalSearchFeature(this IServiceCollection services)
	{
		services.AddScoped<GlobalSearchService>();
		services.AddScoped<IGlobalSearchSource, NoteGlobalSearchSource>();
		services.AddScoped<IGlobalSearchSource, TodoItemGlobalSearchSource>();

		return services;
	}
}

namespace Todo.Service.Features.History;

public static class HistoryServiceCollectionExtensions
{
	public static IServiceCollection RegisterHistoryFeature(this IServiceCollection services)
	{
		services.AddScoped<HistoryService>();
		return services;
	}
}

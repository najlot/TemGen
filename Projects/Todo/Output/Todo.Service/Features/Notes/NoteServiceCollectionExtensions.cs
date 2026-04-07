using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.Notes;

public static class NoteServiceCollectionExtensions
{
	public static IServiceCollection RegisterNoteFeature(this IServiceCollection services)
	{
		services.AddScoped<NoteService>();
		return services;
	}
}

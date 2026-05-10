using Microsoft.Extensions.DependencyInjection;

namespace Todo.Service.Features.Favorites;

public static class FavoriteServiceCollectionExtensions
{
	public static IServiceCollection RegisterFavoritesFeature(this IServiceCollection services)
	{
		services.AddScoped<FavoriteService>();
		services.AddScoped<IFavoriteSource, NoteFavoriteSource>();
		services.AddScoped<IFavoriteSource, TodoItemFavoriteSource>();

		return services;
	}
}

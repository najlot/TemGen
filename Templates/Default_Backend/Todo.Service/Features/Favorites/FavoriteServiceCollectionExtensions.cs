using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.Favorites;

public static class FavoriteServiceCollectionExtensions
{
	public static IServiceCollection RegisterFavoritesFeature(this IServiceCollection services)
	{
		services.AddScoped<FavoriteService>();
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<IFavoriteSource, {definition.Name}FavoriteSource>();");
}
#>
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
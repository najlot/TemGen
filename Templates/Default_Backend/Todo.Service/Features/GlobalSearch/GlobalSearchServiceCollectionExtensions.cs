using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.GlobalSearch;

public static class GlobalSearchServiceCollectionExtensions
{
	public static IServiceCollection RegisterGlobalSearchFeature(this IServiceCollection services)
	{
		services.AddScoped<GlobalSearchService>();
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<IGlobalSearchSource, {definition.Name}GlobalSearchSource>();");
}
#>
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
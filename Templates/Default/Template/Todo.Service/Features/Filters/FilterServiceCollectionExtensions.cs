using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.Filters;

public static class FilterServiceCollectionExtensions
{
	public static IServiceCollection RegisterFiltersFeature(this IServiceCollection services)
	{
		services.AddScoped<FilterService>();
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>
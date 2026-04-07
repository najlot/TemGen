using Microsoft.Extensions.DependencyInjection;

namespace <# Project.Namespace#>.Service.Features.Trash;

public static class TrashServiceCollectionExtensions
{
	public static IServiceCollection RegisterTrashFeature(this IServiceCollection services)
	{
		services.AddScoped<TrashService>();
<#cs foreach (var definition in Definitions.Where(d => !(d.IsEnumeration
	|| d.IsArray
	|| d.IsOwnedType
	|| d.NameLow == "user")).OrderBy(d => d.Name))
{
	WriteLine($"\t\tservices.AddScoped<ITrashSource, {definition.Name}TrashSource>();");
}
#>
		return services;
	}
}
<#cs SetOutputPathAndSkipOtherDefinitions()#>